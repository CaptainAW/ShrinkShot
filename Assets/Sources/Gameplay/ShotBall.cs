using System.Collections;
using UnityEngine;

namespace ShrinkShot.Gameplay
{
    public class ShotBall : MonoBehaviour
    {
        [SerializeField] private float _speed = 14f;

        public float Radius { get; private set; }

        /// <summary>Keeps the growing shot glued to the front of the player ball.</summary>
        public void AttachTo(PlayerBall player, float radius)
        {
            Radius = radius;
            transform.localScale = Vector3.one * (radius * 2f);
            transform.position = new Vector3(0f, radius, player.PathPosition + player.Radius + radius * 0.85f);
        }

        public IEnumerator FlyTo(Vector3 target)
        {
            Vector3 start = transform.position;
            float duration = Vector3.Distance(start, target) / _speed;

            for (float t = 0f; t < duration; t += Time.deltaTime)
            {
                transform.position = Vector3.Lerp(start, target, t / duration);
                yield return null;
            }

            transform.position = target;
        }
    }
}
