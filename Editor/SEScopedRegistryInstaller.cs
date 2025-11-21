// Editor/ScopedRegistryInstaller.cs

using System.IO;
using System.Linq;
using Newtonsoft.Json.Linq;
using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
public static class SEScopedRegistryInstaller
{
    static SEScopedRegistryInstaller()
    {
        AddOpenUPMRegistry();
    }

    private static void AddOpenUPMRegistry()
    {
        string manifestPath = Path.Combine(Application.dataPath, "../Packages/manifest.json");
        var json = File.ReadAllText(manifestPath);
        var manifest = JObject.Parse(json);

        JArray registries = manifest["scopedRegistries"] as JArray ?? new JArray();
        manifest["scopedRegistries"] = registries;

        if (!registries.Any(r => r["name"]?.Value<string>() == "package.openupm.com"))
        {
            JObject reg = JObject.Parse(@"
            {
                'name': 'package.openupm.com',
                'url': 'https://package.openupm.com',
                'scopes': ['com.google.external-dependency-manager']
            }");
            registries.Add(reg);
            File.WriteAllText(manifestPath, manifest.ToString());
            AssetDatabase.Refresh();
        }
    }
}