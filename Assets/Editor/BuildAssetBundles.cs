using UnityEditor;
using System.IO;

public class BuildAssetBundles
{
    public static string pathAssetBundlesOutput = PathConstants.pathStreamingAssetBundlesOutput;

    [MenuItem("Tools/Build AssetBundles")]
    public static void BuildAllAssetBundles()
    {
        string outputPath = pathAssetBundlesOutput;

        if (!Directory.Exists(outputPath))
        {
            Directory.CreateDirectory(outputPath);
        }

        BuildPipeline.BuildAssetBundles(outputPath,
            BuildAssetBundleOptions.None,
            BuildTarget.StandaloneWindows64); // Cambia según tu plataforma si no estás en Windows

        UnityEngine.Debug.Log("✅ AssetBundles generados correctamente.");
    }
}
