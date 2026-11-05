using System.Collections;
using ShrinkShot.Config;
using ShrinkShot.Core;
using ShrinkShot.UI;
using UnityEngine;

namespace ShrinkShot.Gameplay
{
    /// <summary>
    /// Level flow as one coroutine: advance to the first blocker -> hold to charge a shot ->
    /// shot infects obstacles -> repeat until the path is clear -> hop through the door.
    /// Rules live in <see cref="LevelRules"/>.
    /// </summary>
    public class GameController : MonoBehaviour
    {
        private const float AfterShotPause = 0.25f;

        private readonly HoldInput _input = new HoldInput();

        private GameConfig _config;
        private Level _levelPrefab;
        private PlayerBall _playerPrefab;
        private ShotBall _shotPrefab;
        private GameHud _hud;
        private CameraFollow _camera;

        private Level _level;
        private LevelRules _rules;
        private bool[] _alive;
        private Obstacle[] _obstacles;
        private PlayerBall _player;
        private float _volume;

        public void Init(GameConfig config, Level levelPrefab, PlayerBall playerPrefab, ShotBall shotPrefab,
            GameHud hud, CameraFollow cameraFollow)
        {
            _config = config;
            _levelPrefab = levelPrefab;
            _playerPrefab = playerPrefab;
            _shotPrefab = shotPrefab;
            _hud = hud;
            _camera = cameraFollow;

            _hud.PlayAgainClicked += StartLevel;
        }

        public void StartLevel()
        {
            StopAllCoroutines();
            if (_level != null)
                Destroy(_level.gameObject);

            BuildLevel();
            SetVolume(BallMath.Volume(_level.StartRadius));
            _hud.HideResult();

            StartCoroutine(LevelLoop());
        }

        // Ticked before coroutines resume, so LevelLoop sees this frame's input.
        private void Update() => _input.Tick();

        private IEnumerator LevelLoop()
        {
            while (true)
            {
                int blocker = FindBlocker();
                if (blocker < 0)
                {
                    // Hop through the door; it opens by itself when the player comes close.
                    yield return _player.HopTo(_level.PathLength + 3f);
                    _hud.ShowResult(true, string.Empty);
                    yield break;
                }

                if (_volume - _config.MinShotVolume <= _config.CriticalVolume)
                {
                    _hud.ShowResult(false, "Not enough ball left\nto clear the path.");
                    yield break;
                }

                yield return _player.HopTo(_rules.StopPosition(blocker, _player.PathPosition, _player.Radius));
                yield return new WaitUntil(() => _input.PressedThisFrame);

                // Charge: the shot grows out of the player while the hold lasts; total volume is conserved.
                ShotBall shot = Instantiate(_shotPrefab, _level.transform);
                float startVolume = _volume;
                float shotVolume;
                for (float hold = 0f; ; hold += Time.deltaTime)
                {
                    shotVolume = _config.ShotVolumeAt(hold);
                    if (startVolume - shotVolume <= _config.CriticalVolume)
                    {
                        SetVolume(_config.CriticalVolume);
                        _hud.ShowResult(false, "The ball was pumped\ninto the shot completely.");
                        yield break;
                    }

                    SetVolume(startVolume - shotVolume);
                    shot.AttachTo(_player, BallMath.Radius(shotVolume));

                    if (!_input.IsHolding)
                        break;

                    yield return null;
                }

                yield return Fire(shot, shotVolume);
            }
        }

        private IEnumerator Fire(ShotBall shot, float shotVolume)
        {
            // Straight towards the door; the first obstacle on the way takes the hit.
            int target = _rules.FindShotHit(_alive, _player.PathPosition, shot.Radius, out float contactZ);
            yield return shot.FlyTo(new Vector3(0f, shot.Radius, contactZ));

            if (target < 0)
            {
                Destroy(shot.gameObject);
                yield return new WaitForSeconds(AfterShotPause);
                yield break;
            }

            float wait = 0f;
            foreach (Infection infection in _rules.Infect(_alive, target, _rules.InfectionEnergy(shotVolume)))
                wait = Mathf.Max(wait, _obstacles[infection.Index].Infect(infection.Distance));

            Destroy(shot.gameObject);
            yield return new WaitForSeconds(wait + AfterShotPause);
        }

        private void BuildLevel()
        {
            // A fresh instance of the prefab restores every obstacle on restart.
            _level = Instantiate(_levelPrefab);
            _obstacles = _level.CollectObstacles();
            _alive = new bool[_obstacles.Length];

            var positions = new Vector2[_obstacles.Length];
            for (int i = 0; i < _obstacles.Length; i++)
            {
                positions[i] = _level.GroundPosition(_obstacles[i]);
                _alive[i] = true;
            }

            _rules = new LevelRules(_config, positions, _level.PathLength);

            _player = Instantiate(_playerPrefab, _level.transform);
            _player.Place(0f, _level.StartRadius);

            _level.Door.Init(_player.transform);
            _camera.Init(_player.transform, _level.PathLength);
        }

        /// <summary>Single place where the ball size changes: the track shrinks together with the ball.</summary>
        private void SetVolume(float volume)
        {
            _volume = volume;
            _player.SetRadius(BallMath.Radius(volume));
            _level.Path.Set(-1.5f, _level.PathLength, _player.Radius * 2f);
        }

        private int FindBlocker() => _rules.FindFirstBlocker(_alive, _player.PathPosition, _player.Radius);
    }
}
