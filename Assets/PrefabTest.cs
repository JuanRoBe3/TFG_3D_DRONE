using UnityEngine;

public class PrefabTest : MonoBehaviour
{
    [SerializeField] private GameObject prefab;  // arrastra aquí tu CommanderDroneReplica prefab

    void Start()
    {
        Debug.Log("🧪 Test de instanciación comenzado.");

        if (prefab == null)
        {
            Debug.LogError("❌ Prefab no asignado.");
            return;
        }

        try
        {
            var obj = Instantiate(prefab, Vector3.zero, Quaternion.identity);
            Debug.Log("✅ Prefab instanciado correctamente.");

            // Verifica si tiene cámara
            var cam = obj.GetComponentInChildren<Camera>();
            if (cam != null)
            {
                Debug.Log("📷 Cámara detectada en el prefab.");

                if (cam.targetTexture == null)
                {
                    Debug.Log("✅ targetTexture está vacío (como debería).");
                }
                else
                {
                    Debug.LogWarning("⚠️ targetTexture no está vacío. Esto puede causar fallos.");
                }
            }
            else
            {
                Debug.LogWarning("⚠️ No se detectó ninguna cámara en el prefab.");
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogError("💥 Error al instanciar el prefab: " + ex.Message);
        }
    }
}
