using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using System.Threading.Tasks;

public class ResourceSample : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        //LoadResource();
        Addressables.InstantiateAsync("Assets/AssetBundleResources/Prefabs/Cube.prefab");
        //Instantiate("Assets/AssetBundleResources/Prefabs/Cube.prefab");
    }

    private async void LoadResource()
    {
        var loadAssetAsync = Addressables.LoadAssetAsync<GameObject>("Assets/AssetBundleResources/Prefabs/Cube.prefab");
        await loadAssetAsync.Task;
        Instantiate(loadAssetAsync.Result);
    }

    // Update is called once per frame
    void Update()
    {
    }
}
