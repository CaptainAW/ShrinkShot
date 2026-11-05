using ShrinkShot.Core;
using UnityEngine;

namespace ShrinkShot.Config
{
    /// <summary>
    /// Gameplay rules. The level layout and start size live on the level prefab; pure visual tuning
    /// (camera, hop animation, flight and explosion timing) lives on the other prefabs.
    /// </summary>
    [CreateAssetMenu(fileName = "GameConfig", menuName = "ShrinkShot/Game Config")]
    public class GameConfig : ScriptableObject
    {
        [Header("Obstacles")]
        [Tooltip("Collision radius of an obstacle. Keep in sync with the ObstacleMesh asset (modelled for 0.32).")]
        public float ObstacleRadius = 0.32f;

        [Header("Player")]
        [Tooltip("Player radius below which the ball is considered fully pumped into a shot (lose).")]
        public float CriticalRadius = 0.3f;
        [Tooltip("Extra free space on each side of the ball for the path to count as clear.")]
        public float PassMargin = 0.08f;
        [Tooltip("Distance kept in front of the first blocking obstacle when advancing.")]
        public float StopGap = 1.1f;

        [Header("Shot")]
        [Tooltip("Shot volume created instantly on tap.")]
        public float MinShotVolume = 0.1f;
        [Tooltip("Shot volume growth per second of hold.")]
        public float ChargeRate = 0.4f;

        [Header("Infection")]
        [Tooltip("Infection energy = Scale * shotVolume ^ Power. Power > 1 makes big shots pay off in dense clusters, tiny ones only kill a single obstacle.")]
        public float InfectionScale = 9f;
        public float InfectionVolumePower = 1.2f;
        [Tooltip("Max centre distance at which infection can jump between obstacles.")]
        public float InfectionMaxHop = 2.3f;
        [Tooltip("Energy cost of a hop is d^2 / this value: close obstacles infect each other cheaply.")]
        public float InfectionHopReference = 1.05f;

        /// <summary>Player volume at which the ball is lost.</summary>
        public float CriticalVolume => BallMath.Volume(CriticalRadius);

        /// <summary>Shot volume after holding for <paramref name="holdTime"/> seconds.</summary>
        public float ShotVolumeAt(float holdTime) => MinShotVolume + ChargeRate * holdTime;
    }
}
