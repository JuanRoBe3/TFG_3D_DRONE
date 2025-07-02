using UnityEngine;

/// <summary>
/// Fábrica simple de RenderTextures creadas en tiempo de ejecución.
/// Permite indicar resolución (w × h) y profundidad del Z-buffer.
/// </summary>
public static class RTFactory
{
    public static RenderTexture New(int w = 1024, int h = 1024, int depth = 24)
    {
        RenderTexture rt = new RenderTexture(w, h, depth, RenderTextureFormat.ARGB32);
        rt.antiAliasing = 1;                     // 1 = sin MSAA; cámbialo si usas MSAA 2/4
        rt.name = $"RuntimeRT_{w}x{h}";
        rt.Create();
        return rt;
    }
}
