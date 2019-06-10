using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;

public class ResourceBuilder
{
    [MenuItem("Build/AssetBundles/Android")]
    public static void Build_Android()
    {
        var assetBundleList = new List<AssetBundleBuild>();
        var list = new List<string>();
        list.Add("Assets/AssetBundleResources/Prefabs/Cube.prefab");
        AssetBundleBuild build = new AssetBundleBuild();
        build.assetBundleName = "Prefabs/1";
        build.assetNames = list.ToArray();
        assetBundleList.Add(build);

        list.Clear();
        list.Add("Assets/AssetBundleResources/Textures/cat1.png");
        list.Add("Assets/AssetBundleResources/Textures/cat2.png");

        AssetBundleBuild build2 = new AssetBundleBuild();
        build2.assetBundleName = "Texture/1";
        build2.assetNames = list.ToArray();
        assetBundleList.Add(build2);

        var manifest = BuildPipeline.BuildAssetBundles("Assets/Output/", assetBundleList.ToArray(), BuildAssetBundleOptions.ChunkBasedCompression, BuildTarget.Android);
        if (manifest != null)
        {
            Debug.Log("Success build AssetBundle.");
            Display_AssetBundle_Names();
        }
        else
        {
            Debug.LogError("Failed build AssetBundle.");
        }
    }

    [MenuItem("Build/AssetBundles/Display_AssetBundle_Names")]
    public static void Display_AssetBundle_Names()
    {
        foreach (var assetBundleNames in AssetDatabase.GetAllAssetBundleNames())
        {
            Debug.Log($"name: {assetBundleNames}");
        }
    }

    [MenuItem("Build/AssetBundles/Android_Addressable")]
    public static void Build_Android_Addressable()
    {
        // 新規ビルド
        // あらたなcontents_catalogが生成される
        //AddressableAssetSettings.BuildPlayerContent();

        // 更新ビルド
        var path = UnityEditor.AddressableAssets.Build.ContentUpdateScript.GetContentStateDataPath(true);
        if (!string.IsNullOrEmpty(path))
        {
            UnityEditor.AddressableAssets.Build.ContentUpdateScript.BuildContentUpdate(AddressableAssetSettingsDefaultObject.Settings, path);

        }
    }

    [MenuItem("Build/AssetBundles/Addressable_Settings_Test")]
    public static void Addressable_Settings_Test()
    {
        var defaultSettings = AssetDatabase.LoadAssetAtPath<AddressableAssetSettings>("Assets/AddressableAssetsData/AddressableAssetSettings.asset");
        var alterSettings = AssetDatabase.LoadAssetAtPath<AddressableAssetSettings>("Assets/AddressableAssetsData/AddressableAlterSettings.asset");
        var profileId = alterSettings.profileSettings.GetProfileId("RemotePackage");
        if (string.IsNullOrEmpty(profileId))
        {
            Debug.LogError($"profileId is null");
            return;
        }
        alterSettings.activeProfileId = profileId;
        alterSettings.SetDirty(AddressableAssetSettings.ModificationEvent.ActiveProfileSet, alterSettings.activeProfileId, true);

        // 設定切り替え
        EditorBuildSettings.AddConfigObject(AddressableAssetSettingsDefaultObject.kDefaultConfigObjectName, alterSettings, true);
        AddressableAssetSettingsDefaultObject.Settings = alterSettings;
        AddressableAssetSettingsDefaultObject.Settings.activeProfileId = profileId;
         //AddressableAssetSettingsDefaultObject.Settings.groups

        // 新規ビルド
        AddressableAssetSettings.BuildPlayerContent();

        // 設定をデフォルトに戻す
        EditorBuildSettings.AddConfigObject(AddressableAssetSettingsDefaultObject.kDefaultConfigObjectName, defaultSettings, true);
        AddressableAssetSettingsDefaultObject.Settings = defaultSettings;

        // 更新
        //UnityEditor.AddressableAssets.Build.ContentUpdateScript.BuildContentUpdate(alterSettings, "Assets/AddressableAssetsData/Windows/addressables_content_state_alter.bin");

    }

    private void SetDefaultObject()
    {
        var alterSettings = ScriptableObject.Instantiate(AssetDatabase.LoadAssetAtPath<AddressableAssetSettings>("Assets/AddressableAssetsData/AddressableAlterSettings.asset")) as AddressableAssetSettings;
        int i = 0;
        foreach (var s in alterSettings.groups.ToList())
        {
            Debug.Log($"s: {s.name}");
            AddressableAssetSettingsDefaultObject.Settings.groups[i] = s;
            i += 1;
        }
        AddressableAssetSettingsDefaultObject.Settings.SetDirty(AddressableAssetSettings.ModificationEvent.GroupAdded, null, true);
    }

    private void PrintProfile(AddressableAssetSettings settings)
    {
        foreach (var profileName in settings.profileSettings.GetAllProfileNames())
        {
            Debug.Log($"profileName: {profileName}");
        }
        Debug.Log($"settings.activeProfileId: {settings.activeProfileId}");
    }
}
