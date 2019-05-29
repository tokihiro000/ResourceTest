using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.AddressableAssets;

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

    [MenuItem("Build/AssetBundles/Android_Adressable")]
    public static void Build_Android_Adressable()
    {
        UnityEditor.AddressableAssets.Settings.AddressableAssetSettings.BuildPlayerContent();
    }
}
