using UnityEngine.InputSystem;

namespace ShrinkShot.Gameplay
{
    /// <summary>Single "hold" gesture from touch or mouse.</summary>
    public class HoldInput
    {
        public bool IsHolding { get; private set; }
        public bool PressedThisFrame { get; private set; }

        public void Tick()
        {
            bool pressed = IsPressed();
            PressedThisFrame = pressed && !IsHolding;
            IsHolding = pressed;
        }

        private static bool IsPressed()
        {
            var touchscreen = Touchscreen.current;
            if (touchscreen != null && touchscreen.primaryTouch.press.isPressed)
                return true;

            var mouse = Mouse.current;
            return mouse != null && mouse.leftButton.isPressed;
        }
    }
}
