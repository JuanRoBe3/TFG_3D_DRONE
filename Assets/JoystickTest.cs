using UnityEngine;
using UnityEngine.InputSystem;

public class JoystickTest : MonoBehaviour
{
    void Update()
    {
        var joystick = Joystick.current;
        if (joystick == null)
        {
            Debug.LogWarning("❌ No joystick detected.");
            return;
        }

        Vector2 stick = joystick.stick.ReadValue();
        float twist = joystick.twist.ReadValue();
        bool fire = joystick.trigger.isPressed;

        Debug.Log($"🎮 Stick: {stick}, Twist: {twist}, Fire: {fire}");
    }
}
