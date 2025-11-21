using System.Collections;
using System.Collections.Generic;
using System.IO;
using SolarEngine;
using SolarEngineSDK.Editor;
using UnityEditor;
using UnityEditor.PackageManager;
using UnityEngine;
[InitializeOnLoad]
public class SolorEnginePackageManager : MonoBehaviour
{

    private static readonly string _packageName = "solarengine-unity-sdk";
    static SolorEnginePackageManager()
    {
        AssetDatabase.importPackageCompleted += OnImportFinishHandle;
    }
    static void OnImportFinishHandle(string packageName)
    {
        if (packageName ==_packageName)
        {
            finishHandle(false);
            cleanupOpenHarmonyBridge();
        }

    }
    

    static void finishHandle()
    {
     
        
     
    }

  
    
    static void finishHandle(bool isShow=false)
    {

        ApplySetting._applySetting(isShow);
    }
    

    static void cleanupOpenHarmonyBridge()
    {
        const string CLEANUP_KEY = "SolarEngine_OpenHarmonyBridge_Cleaned";

        
        if (EditorPrefs.GetBool(CLEANUP_KEY, false))
        {
            Debug.LogWarning("[SolarEngine] cleanupOpenHarmonyBridge skipped (already cleaned).");
            return;
        }
        string root = Path.Combine(Application.dataPath, "Plugins/OpenHarmony/SolarEngine");

        string[] deleteTargets =
        {
            "RemoteConfig/RCNativeBridge.etslib",
            "RemoteConfig/SERCOpenHarmonyProxy.ets",
            "SENativeBridge.etslib",
            "SEOpenHarmonyProxy.ets"
        };

        foreach (var target in deleteTargets)
        {
            // 拼接完整路径
            string fullPath = Path.Combine(root, target);

            // 统一处理一下路径分隔符
            fullPath = fullPath.Replace("\\", "/");

            if (File.Exists(fullPath))
            {
                try
                {
                    File.Delete(fullPath);
                    Debug.LogWarning($"[SolarEngine] ✅ Deleted: {fullPath}");
                }
                catch (System.Exception e)
                {
                    Debug.LogError($"[SolarEngine] ❌ Failed delete: {fullPath}\n{e}");
                }
            }
            else
            {
                Debug.LogWarning($"[SolarEngine] ⚠ Not Found: {fullPath}");
            }
        }
        EditorPrefs.SetBool(CLEANUP_KEY, true);
        AssetDatabase.Refresh();
    }

    
    [MenuItem("SolarEngineSDK/Documentation/UnityDocumentation", false, 0)]
    static void unityDocumentation()
    {
        Application.OpenURL("https://help.solar-engine.com/cn/docs/51FROeEQ");
    }
    
    [MenuItem(ConstString.MenuItem.iOSChangelog, false, 0)]
    static void solarEngineDocsiOS()
    {
        Application.OpenURL("https://help.solar-engine.com/cn/docs/geng-xin-ri-zhi-RAvv");
    }
    [MenuItem(ConstString.MenuItem.androidChangelog, false, 0)]
    static void solarEngineDocsAndroid()
    {
        Application.OpenURL("https://help.solar-engine.com/cn/docs/geng-xin-ri-zhi");
    }
    
    private const string storageWarning = "You can only choose either China or Overseas！";
    private const string nostorageWarning = "You must choose either China or Overseas!";
  

}

[InitializeOnLoad]
public static class SDKInstallChecker
{
    private const string PackageName = "com.solarengine.sdk";
    private const string SESDKUPMImportedKey = "SESDKUPMImported";

    private static bool _checked;

    static SDKInstallChecker()
    {
        if (_checked) return;
        _checked = true;

        if (!EditorPrefs.GetBool(SESDKUPMImportedKey, false))
        {
            if (PackageChecker.IsUPMPackageInstalled())
            {
                ImportConfig();
                EditorPrefs.SetBool(SESDKUPMImportedKey, true);
            }
         
         
            
        }
    }
    [MenuItem("SolarEngineSDK/import configuration module")]
    public static void ImportConfig()
    {
        ImportPackage("solarengine-unity-sdk-upm.unitypackage");
    }
    private static void ImportPackage(string fileName)
    {
        string packagePath = $"Packages/{PackageName}/~PackagesContent/{fileName}";

        if (!File.Exists(packagePath))
        {
            Debug.LogError($"File {fileName}not found. The current SDK may not be imported by UPM.");
            return;
        }

        AssetDatabase.ImportPackage(packagePath, true);
        Debug.Log($"The  package {fileName} has been imported");
    }
}

#if UNITY_EDITOR


public static class PackageChecker
{
    private static string packagePath = "";
    public static bool IsUPMPackageInstalled(string packageName="com.solarengine.sdk")
    {
        var listRequest = Client.List(true, false);
        while (!listRequest.IsCompleted) {} // 等待完成

        if (listRequest.Status == StatusCode.Success)
        {
            foreach (var pkg in listRequest.Result)
            {

                if (pkg.name == packageName)
                {
                    packagePath = pkg.resolvedPath;
                    return true;
                }
                   
            }
        }
        return false;
    }

    public static string GetPackagePath()
    {
        return packagePath;
    }
    
}
#endif
