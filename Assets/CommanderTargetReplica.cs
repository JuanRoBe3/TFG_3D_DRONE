using UnityEngine;

/// <summary>
/// Representación visual de un target en la escena del Comandante.
/// </summary>
public class CommanderTargetReplica : MonoBehaviour
{
    [Header("Componentes del prefab")]
    [SerializeField] private ClickableTarget clicker;     // Script que permite clicarlo
    [SerializeField] private Transform visualTransform;   // Parte visual para rotación u otros efectos

    private string targetId;

    /// <summary>
    /// Inicializa la réplica del target con los datos recibidos.
    /// </summary>
    public void Init(string id)
    {
        if (string.IsNullOrEmpty(id))
        {
            Debug.LogError("❌ CommanderTargetReplica.Init → ID inválido");
            return;
        }

        targetId = id;

        // 1️⃣ Asignar ID al ClickableTarget
        if (clicker == null)
            clicker = GetComponentInChildren<ClickableTarget>();

        if (clicker != null)
        {
            clicker.SetTargetId(targetId);
        }
        else
        {
            Debug.LogWarning($"⚠️ No se encontró ClickableTarget en la réplica de {targetId}");
        }

        // 2️⃣ (Opcional) personalizar visual
        if (visualTransform != null)
        {
            visualTransform.localScale = Vector3.one * 1.5f; // Ejemplo: escalar el marcador
        }

        Debug.Log($"📌 Réplica visual del target {targetId} inicializada.");
    }

    /// <summary>
    /// Acceso externo al ID del target.
    /// </summary>
    public string GetTargetId()
    {
        return targetId;
    }
}
