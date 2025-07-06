using UnityEngine;

/// <summary>
/// Hace que este objeto siga a otro (posici�n y rotaci�n Y),
/// manteniendo el objeto tumbado para ser visto desde una c�mara cenital.
/// </summary>
public class FollowAndRotate : MonoBehaviour
{
    private Transform target; // No editable desde el Inspector: se asigna por c�digo

    /// <summary>
    /// Asigna el objeto que se quiere seguir.
    /// Debe llamarse desde el spawner justo despu�s de instanciar la flecha.
    /// </summary>
    public void SetTarget(Transform t)
    {
        target = t;
    }

    private void LateUpdate()
    {
        if (target == null) return;

        // Posici�n completa (X, Y, Z)
        transform.position = target.position;

        // Rotaci�n solo en Y (yaw), pero tumbado 90� en X para que se vea como plano desde arriba
        Vector3 euler = target.eulerAngles;
        transform.rotation = Quaternion.Euler(90f, euler.y, 0);
    }
}
