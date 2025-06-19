using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

public class JoystickButtonScanner : MonoBehaviour
{
    void Update()
    {
        var joystick = Joystick.current;
        if (joystick == null) return;

        for (int i = 0; i < 20; i++)
        {
            var button = joystick.TryGetChildControl<ButtonControl>($"button{i}");
            if (button != null && button.isPressed)
            {
                Debug.Log($"🛰️ Botón {i} presionado");
            }
        }
    }
}
