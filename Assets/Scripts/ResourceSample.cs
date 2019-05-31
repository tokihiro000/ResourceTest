using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using System.Threading.Tasks;
using UniRx;

public class ResourceSample : MonoBehaviour
{
    private Dictionary<string, GameObject> gameObjDict = new Dictionary<string, GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine("LoadEnumerator");
        //LoadResource();
        //Addressables.InstantiateAsync("Assets/AssetBundleResources/Prefabs/Cube.prefab");
        //UnityEngine.ResourceManagement.ResourceManager;
    }

    private async void LoadResource()
    {
        //var loadAssetAsync = Addressables.LoadAssetAsync<GameObject>("Assets/AssetBundleResources/Prefabs/Cube.prefab");
        //await loadAssetAsync.Task;
        //Instantiate(loadAssetAsync.Result);
    }

    private IEnumerator LoadEnumerator()
    {
        // 下記はload.Taskがnullのためエラーになる
        //var load = Addressables.DownloadDependenciesAsync("Assets/AssetBundleResources/Prefabs/Cube.prefab");
        //Debug.Log($"load: {load.ToString()}");
        //await load.Task;
        // donwload
        var downloadSizeHandle = Addressables.GetDownloadSizeAsync("Assets/AssetBundleResources/Prefabs/Cube.prefab");
        yield return downloadSizeHandle;
        if (downloadSizeHandle.Status == AsyncOperationStatus.Succeeded)
        {
            var downloadSize = downloadSizeHandle.Result;
            Debug.Log($"[AsyncOperationStatus: Succeeded] donwloadSize: {downloadSize}");
        }
        else
        {
            Debug.Log($"[AsyncOperationStatus: {downloadSizeHandle.Status.ToString()}]");
        }

        var handle = Addressables.DownloadDependenciesAsync("Assets/AssetBundleResources/Prefabs/Cube.prefab");
        yield return handle;
        if (handle.Status == AsyncOperationStatus.Succeeded)
        {
            Debug.Log($"[AsyncOperationStatus: Succeeded]");
            foreach (var resource in handle.Result as System.Collections.Generic.List<UnityEngine.ResourceManagement.ResourceProviders.IAssetBundleResource>)
            {
                Debug.Log($"[AsyncOperationStatus: Succeeded] {resource.GetAssetBundle().ToString()}");
            }
        }
        else
        {
            Debug.Log($"[AsyncOperationStatus: {handle.Status.ToString()}]");
        }
        Addressables.Release(handle);

        // load
        var path = "Assets/AssetBundleResources/Prefabs/Cube.prefab";
        var obj = Addressables.LoadAssetAsync<GameObject>(path);
        yield return obj;
        gameObjDict[path] = obj.Result;

        // Instantiate
        Instantiate(gameObjDict[path]);
    }

    // Update is called once per frame
    void Update()
    {
    }
}
