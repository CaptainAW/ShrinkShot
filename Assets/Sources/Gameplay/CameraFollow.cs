using UnityEngine;

namespace ShrinkShot.Gameplay
{
    /// <summary>
    /// Follows the ball from behind and to the left, so the ball sits in the lower-left part
    /// of the screen and the door is up and to the right.
    /// </summary>
    [RequireComponent(typeof(Camera))]
    public class CameraFollow : MonoBehaviour
    {
        [SerializeField] private Vector3 _offset = new Vector3(-3.2f, 15f, -8.5f);
        [SerializeField] private float _lookAhead = 7f;
        [SerializeField] private float _lookSide = 1.2f;
        [SerializeField] private float _fov = 55f;

        private Transform _target;
        private float _maxFocusZ = float.MaxValue;

        /// <param name="pathEnd">Z of the door; the camera stops so the door stays in view.</param>
        public void Init(Transform target, float pathEnd)
        {
            _target = target;
            _maxFocusZ = pathEnd - _lookAhead + 1f;
            SnapTo(target.position.z);
        }

        private void SnapTo(float z)
        {
            GetComponent<Camera>().fieldOfView = _fov;

            var focus = new Vector3(0f, 0f, Mathf.Min(z, _maxFocusZ));
            transform.position = focus + _offset;
            transform.LookAt(focus + new Vector3(_lookSide, 0f, _lookAhead));
        }

        private void LateUpdate()
        {
            if (_target == null) return;
            SnapTo(_target.position.z);
        }
    }
}