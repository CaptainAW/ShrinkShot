using System.Collections;
using UnityEngine;

namespace ShrinkShot.Gameplay
{
    /// <summary>Visual of the player ball. Logic position is the distance along the path (Z).</summary>
    public class PlayerBall : MonoBehaviour
    {
        [SerializeField] private float _moveSpeed = 4.5f;
        [SerializeField] private float _hopLength = 1.4f;
        [SerializeField] private float _hopHeight = 0.45f;

        private float _hop;

        public float Radius { get; private set; }
        public float PathPosition { get; private set; }

        public void Place(float pathPosition, float radius)
        {
            PathPosition = pathPosition;
            SetRadius(radius);
        }

        public void SetRadius(float radius)
        {
            Radius = radius;
            transform.localScale = Vector3.one * (radius * 2f);
            UpdatePosition();
        }

        /// <summary>Bounces along the path centre to the target position.</summary>
        public IEnumerator HopTo(float target)
        {
            float start = PathPosition;
            float distance = target - start;
            if (distance <= 0.01f)
                yield break;

            // Whole number of hops so the ball lands exactly on the target.
            int hops = Mathf.Max(1, Mathf.RoundToInt(distance / _hopLength));
            float duration = distance / _moveSpeed;

            for (float t = 0f; t < duration; t += Time.deltaTime)
            {
                float k = t / duration;
                PathPosition = Mathf.Lerp(start, target, k);
                _hop = _hopHeight * Mathf.Abs(Mathf.Sin(k * hops * Mathf.PI));
                UpdatePosition();
                yield return null;
            }

            PathPosition = target;
            _hop = 0f;
            UpdatePosition();
        }

        private void UpdatePosition()
        {
            transform.position = new Vector3(0f, Radius + _hop, PathPosition);
        }
    }
}
