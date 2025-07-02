using System.Collections.Generic;
using UnityEngine;

public static class RenderTextureRegistry
{
    private static readonly Dictionary<string, RenderTexture> map = new();

    /// <summary> Devuelve una RT existente o crea una nueva con RTFactory.New() </summary>
    public static RenderTexture GetOrCreate(string id, int w = 1024, int h = 1024)
    {
        if (!map.TryGetValue(id, out var rt))
        {
            rt = RTFactory.New(w, h);   // ⬅️ Único sitio donde se instancia
            map[id] = rt;
        }
        return rt;
    }

    public static RenderTexture Get(string id) => map.TryGetValue(id, out var rt) ? rt : null;
}
