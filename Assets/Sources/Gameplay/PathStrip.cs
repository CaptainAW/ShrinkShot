using UnityEngine;

namespace ShrinkShot.Gameplay
{
    /// <summary>The track on the ground; its width follows the ball diameter.</summary>
    public class PathStrip : MonoBehaviour
    {
        private const float GroundOffset = 0.01f;

        public void Set(float fromZ, float toZ, float width)
        {
            float length = Mathf.Max(0.01f, toZ - fromZ);
            transform.position = new Vector3(0f, GroundOffset, (fromZ + toZ) * 0.5f);
            transform.localScale = new Vector3(width, 1f, length);
        }
    }
}
