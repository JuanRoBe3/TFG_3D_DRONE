using UnityEngine;
using UnityEngine.UI;

public class DebugRawImageChecker : MonoBehaviour
{
    public RawImage image;

    void Update()
    {
        if (image.texture != null)
        {
            Debug.Log($"🖼️ RawImage tiene texture activa: {image.texture.name}");
        }
        else
        {
            Debug.LogWarning("⚠️ RawImage NO tiene texture activa.");
        }
    }
}
