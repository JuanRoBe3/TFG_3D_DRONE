using UnityEngine;

/// <summary>
/// Hace que este objeto siga a otro (posición y rotación Y),
/// manteniendo el objeto tumbado para ser visto desde una cámara cenital.
/// </summary>
public class FollowAndRotate : MonoBehaviour
{
    private Transform target; // No editable desde el Inspector: se asigna por código

    /// <summary>
    /// Asigna el objeto que se quiere seguir.
    /// Debe llamarse desde el spawner justo después de instanciar la flecha.
    /// </summary>
    public void SetTarget(Transform t)
    {
        target = t;
    }

    private void LateUpdate()
    {
        if (target == null) return;

        // Posición completa (X, Y, Z)
        transform.position = target.position;

        // Rotación solo en Y (yaw), pero tumbado 90° en X para que se vea como plano desde arriba
        Vector3 euler = target.eulerAngles;
        transform.rotation = Quaternion.Euler(90f, euler.y, 0);
    }
}
