using UnityEngine;

/// <summary>
/// Script que detecta clics en el marcador del target y muestra el zoom.
/// </summary>
public class ClickableTarget : MonoBehaviour
{
    [SerializeField] private string targetId;

    public void SetTargetId(string id) => targetId = id;
    public string GetTargetId() => targetId;

    /// <summary>
    /// Llama al visor de zoom con el ID del target.
    /// </summary>
    public void TriggerZoomView()
    {
        Debug.Log($"📸 Click en target: {targetId}");

        if (string.IsNullOrEmpty(targetId))
        {
            Debug.LogWarning("⚠️ ClickableTarget sin ID");
            return;
        }

        // ✅ Mostrar zoom usando el sistema centralizado
        ZoomedTargetViewPanelManager.ShowTargetView(targetId);
    }

    /// <summary>
    /// Detecta clic del usuario (requiere Collider y cámara que lo vea).
    /// </summary>
    private void OnMouseDown()
    {
        TriggerZoomView();
    }
}
