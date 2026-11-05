using ShrinkShot.Config;
using ShrinkShot.Gameplay;
using ShrinkShot.UI;
using UnityEngine;

namespace ShrinkShot
{
    /// <summary>
    /// Composition root: every dependency is assigned in the inspector and handed to
    /// <see cref="GameController"/> from here; nothing in the game looks objects up in the scene.
    /// </summary>
    public class GameBootstrapper : MonoBehaviour
    {
        [Header("Assets")]
        [SerializeField] private GameConfig _config;
        [SerializeField] private Level _level;
        [SerializeField] private PlayerBall _player;
        [SerializeField] private ShotBall _shot;

        [Header("Scene")]
        [SerializeField] private GameController _controller;
        [SerializeField] private CameraFollow _camera;

        private void Awake()
        {
            Application.targetFrameRate = 60;
            Screen.sleepTimeout = SleepTimeout.NeverSleep;

            if (_config == null || _level == null || _player == null || _shot == null
                || _controller == null || _camera == null)
            {
                Debug.LogError("[ShrinkShot] Bootstrapper is not set up: assign the config, the prefabs, the controller and the camera in the inspector.");
                enabled = false;
                return;
            }

            GameHud hud = GameHud.Create();

            _controller.Init(_config, _level, _player, _shot, hud, _camera);
            _controller.StartLevel();
        }
    }
}
