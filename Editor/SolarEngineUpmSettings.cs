using System.IO;
using System.Linq;
using Newtonsoft.Json.Linq;
using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
public static class SolarEngineUpmSettings
{
    private const string PackageName = "com.solarengine.sdk";

    // ✅ 项目级持久标记文件
    private const string InstallFlagPath = "Library/SolarEngine/SESDK_UPM_IMPORTED.flag";

    private static bool _checked;

    static SolarEngineUpmSettings()
    {
        AddOpenUPMRegistry();
        ChackAndImportConfig();
    }

    [MenuItem("SolarEngineSDK/UPM/Add Registry")]
    public static void AddOpenUPMRegistry()
    {
        string manifestPath = Path.Combine(Application.dataPath, "../Packages/manifest.json");
        var json = File.ReadAllText(manifestPath);
        var manifest = JObject.Parse(json);

        JArray registries = manifest["scopedRegistries"] as JArray;
        if (registries == null)
        {
            registries = new JArray();
            manifest["scopedRegistries"] = registries;
        }

        if (!registries.Any(r => r["name"]?.Value<string>() == "package.openupm.com"))
        {
            JObject reg = new JObject
            {
                ["name"] = "package.openupm.com",
                ["url"] = "https://package.openupm.com",
                ["scopes"] = new JArray("com.google.external-dependency-manager")
            };

            registries.Add(reg);
            File.WriteAllText(manifestPath, manifest.ToString(Newtonsoft.Json.Formatting.Indented));
            AssetDatabase.Refresh();
            Debug.Log("[SE] Added OpenUPM registry successfully.");
        }
        // else
        // {
        //     Debug.Log("[SE] OpenUPM registry already exists.");
        // }
    }

    private static void ChackAndImportConfig()
    {
        if (_checked) return;
        _checked = true;

        bool imported = File.Exists(InstallFlagPath);
        if (imported) return;
        Debug.LogWarning("[SolarEngine] SESDKInstallChecker (Project Flag) = " + imported);


        ImportConfig();
        CreateInstallFlag();
    }

    /// <summary>
    /// 创建项目级标记文件
    /// </summary>
    private static void CreateInstallFlag()
    {
        string folder = Path.GetDirectoryName(InstallFlagPath);
        if (!Directory.Exists(folder))
            Directory.CreateDirectory(folder);

        File.WriteAllText(InstallFlagPath, System.DateTime.Now.ToString());
    }

    [MenuItem("SolarEngineSDK/UPM/Import Config")]
    public static void ImportConfig()
    {
        ImportPackage("solarengine-unity-sdk-upm.unitypackage");
    }

    private static void ImportPackage(string fileName)
    {
        Debug.LogWarning("Importing package...");

        string packagePath = $"Packages/{PackageName}/~PackagesContent/{fileName}";

        if (!File.Exists(packagePath))
        {
            Debug.LogError($"File {fileName} not found. The current SDK may not be imported by UPM.");
            return;
        }

        AssetDatabase.ImportPackage(packagePath, true);
        Debug.Log($"The package {fileName} has been imported.");
    }
}