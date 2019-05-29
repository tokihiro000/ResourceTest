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
        LoadResource();
    }

    private async void LoadResource()
    {
        var loadAssetAsync = Addressables.LoadAssetAsync<GameObject>("Assets/AssetBundleResources/Prefabs/Cube2.prefab");
        await loadAssetAsync.Task;
        Debug.Log("Load Complete");
        Instantiate(loadAssetAsync.Result);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
