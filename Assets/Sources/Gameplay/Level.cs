using UnityEngine;

namespace ShrinkShot.Gameplay
{
    /// <summary>
    /// Hand-authored level prefab. The path runs along local +Z from the player start (prefab origin)
    /// to the door; every <see cref="Obstacle"/> under the obstacles root takes part in the game.
    /// </summary>
    public class Level : MonoBehaviour
    {
        [Tooltip("Player radius at the start. Must cover clearing the path + the critical size with a 20% reserve.")]
        [SerializeField] private float _startRadius = 1f;
        [SerializeField] private Door _door;
        [SerializeField] private PathStrip _path;
        [SerializeField] private Transform _obstaclesRoot;

        public float StartRadius => _startRadius;
        public Door Door => _door;
        public PathStrip Path => _path;
        public float PathLength => _door.transform.localPosition.z;

        public Obstacle[] CollectObstacles() => _obstaclesRoot.GetComponentsInChildren<Obstacle>();

        /// <summary>Obstacle position on the ground plane as (x, z), relative to the level origin.</summary>
        public Vector2 GroundPosition(Obstacle obstacle)
        {
            Vector3 p = transform.InverseTransformPoint(obstacle.transform.position);
            return new Vector2(p.x, p.z);
        }
    }
}
