using UnityEngine;

public class RenderTextureRegistry : MonoBehaviour
{
    public RenderTexture firstPersonTexture;           // Para el piloto
    public RenderTexture topDownTexture;

    public RenderTexture commanderFirstPersonTexture;  // Para el comandante
    public RenderTexture commanderTopDownTexture;

    public static RenderTextureRegistry Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        EnsureRenderTextures();
    }

    private void EnsureRenderTextures()
    {
        // Si te olvidas de asignarlos, los creamos aquí con resolución 512x512 por defecto
        if (firstPersonTexture == null)
            firstPersonTexture = CreateRenderTexture("PilotFPTexture");

        if (topDownTexture == null)
            topDownTexture = CreateRenderTexture("PilotTDTexture");

        if (commanderFirstPersonTexture == null)
            commanderFirstPersonTexture = CreateRenderTexture("CmdFPTexture");

        if (commanderTopDownTexture == null)
            commanderTopDownTexture = CreateRenderTexture("CmdTDTexture");
    }

    private RenderTexture CreateRenderTexture(string name)
    {
        var rt = new RenderTexture(512, 512, 16); // resolución, profundidad
        rt.name = name;
        rt.Create();
        return rt;
    }
}
