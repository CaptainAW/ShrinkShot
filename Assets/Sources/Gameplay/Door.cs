using UnityEngine;

namespace ShrinkShot.Gameplay
{
    /// <summary>Double door that opens once the player comes close enough.</summary>
    public class Door : MonoBehaviour
    {
        [SerializeField] private Transform _leftLeaf;
        [SerializeField] private Transform _rightLeaf;
        [SerializeField] private float _openDistance = 5f;
        [SerializeField] private float _openAngle = 105f;

        private Transform _player;

        public void Init(Transform player)
        {
            _player = player;
            SetAngle(0f);
            enabled = true;
        }

        private void Update()
        {
            Vector3 toPlayer = _player.position - transform.position;
            toPlayer.y = 0f;
            if (toPlayer.magnitude > _openDistance)
                return;

            SetAngle(_openAngle);
            enabled = false;
        }

        private void SetAngle(float angle)
        {
            _leftLeaf.localRotation = Quaternion.Euler(0f, -angle, 0f);
            _rightLeaf.localRotation = Quaternion.Euler(0f, angle, 0f);
        }
    }
}
