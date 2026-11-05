using UnityEngine;

namespace ShrinkShot.Core
{
    /// <summary>Balls trade volume, not radius: player volume + shot volume stays constant while charging.</summary>
    public static class BallMath
    {
        private const float VolumeFactor = 4f / 3f * Mathf.PI;

        public static float Volume(float radius) => VolumeFactor * radius * radius * radius;

        public static float Radius(float volume) => volume <= 0f ? 0f : Mathf.Pow(volume / VolumeFactor, 1f / 3f);
    }
}
