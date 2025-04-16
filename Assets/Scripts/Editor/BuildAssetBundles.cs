using UnityEditor;
using System.IO;
using UnityEngine;

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
            BuildTarget.StandaloneWindows64);

        RenameAssetBundlesToAddExtension(outputPath);

        Debug.Log("✅ AssetBundles generados y renombrados con extensión .bundle.");
    }

    [MenuItem("Tools/Build AssetBundles (Clean)")]
    public static void BuildCleanAssetBundles()
    {
        string outputPath = pathAssetBundlesOutput;

        if (Directory.Exists(outputPath))
            Directory.Delete(outputPath, true);

        Directory.CreateDirectory(outputPath);

        BuildPipeline.BuildAssetBundles(outputPath,
            BuildAssetBundleOptions.None,
            BuildTarget.StandaloneWindows64);

        RenameAssetBundlesToAddExtension(outputPath);

        Debug.Log("🧽 AssetBundles limpiados, regenerados y renombrados.");
    }

    private static void RenameAssetBundlesToAddExtension(string folderPath)
    {
        string[] files = Directory.GetFiles(folderPath);

        foreach (string file in files)
        {
            string fileName = Path.GetFileName(file);

            // Ignorar el manifest general y meta
            if (fileName == Path.GetFileName(folderPath) ||
                fileName.EndsWith(".manifest") ||
                fileName.EndsWith(".meta") ||
                fileName.EndsWith(".bundle"))
                continue;

            string newPath = Path.Combine(folderPath, fileName + ".bundle");

            // Si ya existe, eliminar
            if (File.Exists(newPath))
                File.Delete(newPath);

            File.Move(file, newPath);
            Debug.Log($"📦 Renombrado: {fileName} → {fileName}.bundle");
        }
    }
}
