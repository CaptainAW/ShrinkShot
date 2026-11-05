using System.Collections;
using UnityEngine;

namespace ShrinkShot.Gameplay
{
    public class Obstacle : MonoBehaviour
    {
        [SerializeField] private Renderer _renderer;
        [SerializeField] private Material _infectedMaterial;
        [SerializeField] private ParticleSystem _explosionPrefab;
        [SerializeField] private float _explosionHeight = 0.5f;
        [Tooltip("Infection energy spread per second (visual timing only).")]
        [SerializeField] private float _spreadSpeed = 7f;
        [Tooltip("Time an obstacle stays infected before it explodes.")]
        [SerializeField] private float _explodeDelay = 0.35f;

        /// <param name="spreadDistance">Infection energy spent to reach this obstacle from the hit one.</param>
        /// <returns>Seconds until the obstacle explodes.</returns>
        public float Infect(float spreadDistance)
        {
            float delay = spreadDistance / _spreadSpeed;
            StartCoroutine(InfectRoutine(delay));
            return delay + _explodeDelay;
        }

        private IEnumerator InfectRoutine(float delay)
        {
            yield return new WaitForSeconds(delay);
            _renderer.sharedMaterial = _infectedMaterial;
            yield return new WaitForSeconds(_explodeDelay);

            if (_explosionPrefab != null)
                Instantiate(_explosionPrefab, transform.position + Vector3.up * _explosionHeight, Quaternion.identity);

            gameObject.SetActive(false);
        }
    }
}
