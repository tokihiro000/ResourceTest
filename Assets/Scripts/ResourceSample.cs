using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.AddressableAssets.ResourceProviders;
using UnityEngine.AddressableAssets.ResourceLocators;
using UnityEngine.ResourceManagement.ResourceLocations;
using UnityEngine.ResourceManagement.ResourceProviders;
using System.Threading.Tasks;
using UniRx;
using UnityEngine.AddressableAssets.Initialization;

public class ResourceSample : MonoBehaviour
{
    public Button cube1, cube2;
    private Dictionary<string, GameObject> gameObjDict = new Dictionary<string, GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        //StartCoroutine("LoadEnumerator");
        //LoadResource();
        //Addressables.InstantiateAsync("Assets/AssetBundleResources/Prefabs/Cube.prefab");
        //UnityEngine.ResourceManagement.ResourceManager;
        //Caching.IsVersionCached
        cube1?.OnClickAsObservable().Subscribe(x =>
        {
            //StartCoroutine("LoadEnumerator", "Assets/AssetBundleResources/Prefabs/Cube.prefab");
            List<string> locations = new List<string>();
            locations.Add("Assets/AssetBundleResources/Prefabs/Cube2.prefab");
            StartCoroutine("LoadSizeEnumerator", locations);
        });

        cube2?.OnClickAsObservable().Subscribe(x =>
        {
            StartCoroutine("LoadEnumerator", "Assets/AssetBundleResources/Prefabs/Cube2.prefab");
        });
    }

    private async void LoadResource()
    {
        //var loadAssetAsync = Addressables.LoadAssetAsync<GameObject>("Assets/AssetBundleResources/Prefabs/Cube.prefab");
        //await loadAssetAsync.Task;
        //Instantiate(loadAssetAsync.Result);
    }

    private IEnumerator LoadSizeEnumerator(IList<string> locations)
    {
        // サイズ取得
        var downloadSizeHandle = Addressables.GetDownloadSizeAsync(locations);
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
    }

    private IEnumerator LoadEnumerator(string assetsPath)
    {
        // ビルドパス
        //var catalogPath = Addressables.BuildPath + "/catalog.json";



        //var initOp = new InitializationOperation(aa);
        //initOp.m_rtdOp = aa.ResourceManager.ProvideResource<ResourceManagerRuntimeData>(runtimeDataLocation);

        //Debug.Log($"catalogPath: {UnityEngine.AddressableAssets.Initialization.ResourceManagerRuntimeData.kCatalogAddress}");
        //Addressables.LoadContentCatalogAsync
        //var contentCatalogPath = Addressables.ResolveInternalId(PlayerPrefs.GetString(Addressables.kAddressablesRuntimeDataPath, Addressables.RuntimePath + "/catalog.json"));
        //Debug.Log($"contentCatalogPath: {contentCatalogPath}");
        //var catalogHandle = Addressables.LoadContentCatalogAsync($"{System.IO.Path.Combine(Addressables.RuntimePath, "catalog.json")}");
        //yield return catalogHandle;

        //var catalogHandle = Addressables.LoadContentCatalogAsync("http://172.22.109.122:55560/catalog_2019.06.03.02.47.53.json");
        //yield return catalogHandle;
        //Addressables.ResourceLocators.Add(catalogHandle.Result);
        //Debug.Log($"Addressables.BuildPath: {Addressables.BuildPath}");
        //Debug.Log($"Addressables.RuntimePath: {Addressables.RuntimePath}");
        //Debug.Log($"Addressables.PlayerBuildDataPath: {Addressables.PlayerBuildDataPath}");
        //Debug.Log($"Addressables.kAddressablesRuntimeDataPath: {Addressables.kAddressablesRuntimeDataPath}");

        // サイズ取得
        var downloadSizeHandle = Addressables.GetDownloadSizeAsync(assetsPath);
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

        // ダウンロード
        // 下記はload.Taskがnullのためエラーになる
        //var load = Addressables.DownloadDependenciesAsync("Assets/AssetBundleResources/Prefabs/Cube.prefab");
        //Debug.Log($"load: {load.ToString()}");
        //await load.Task;
        var handle = Addressables.DownloadDependenciesAsync(assetsPath);
        yield return handle;
        if (handle.Status == AsyncOperationStatus.Succeeded)
        {
            Debug.Log($"[AsyncOperationStatus: Succeeded]");
            foreach (var resource in handle.Result as System.Collections.Generic.List<UnityEngine.ResourceManagement.ResourceProviders.IAssetBundleResource>)
            {
                // AssetBundleファイル
                AssetBundle ab = resource.GetAssetBundle();
                Debug.Log($"[AsyncOperationStatus: Succeeded] {resource.GetAssetBundle()}");
            }
        }
        else
        {
            Debug.Log($"[AsyncOperationStatus: {handle.Status.ToString()}]");
        }
        Addressables.Release(handle);

        // load
        var path = assetsPath;
        var obj = Addressables.LoadAssetAsync<GameObject>(path);
        yield return obj;
        gameObjDict[path] = obj.Result;

        // Instantiate
        Instantiate(gameObjDict[path]);

        IResourceLocation[] dependencies = new IResourceLocation[(int)ContentCatalogProvider.DependencyHashIndex.Count];

        //var location = new ResourceLocationBase(k_LocationName, k_LocationId, typeof(ContentCatalogProvider).FullName, dependencies);
        var playerSettingsLocation = Addressables.ResolveInternalId(PlayerPrefs.GetString(Addressables.kAddressablesRuntimeDataPath, Addressables.RuntimePath + "/settings.json"));
        var runtimeDataLocation = new ResourceLocationBase("RuntimeData", playerSettingsLocation, typeof(JsonAssetProvider).FullName);
        var resourceManager = Addressables.ResourceManager;
        var catalogHandle = resourceManager.ProvideResource<ResourceManagerRuntimeData>(runtimeDataLocation);
        yield return catalogHandle;
        var rtd = catalogHandle.Result;
        if (rtd != null)
        {
            foreach (var loc in rtd.CatalogLocations)
            {
                //Debug.Log($"location: {loc}");
            }

            var locMap = new ResourceLocationMap(rtd.CatalogLocations);
            IList<IResourceLocation> localCatalogList;
            if (!locMap.Locate(ResourceManagerRuntimeData.kCatalogAddress, out localCatalogList))
            {
                Debug.LogError($"not found: {ResourceManagerRuntimeData.kCatalogAddress}");
            }
            else
            {
                Debug.Log($"catalogs.Count: {localCatalogList.Count}");
                var catalog = localCatalogList[0];
                Debug.Log($"internalId: {catalog.InternalId}");
                Debug.Log($"provideId: {catalog.ProviderId.ToString()}");
                dependencies[(int)ContentCatalogProvider.DependencyHashIndex.Cache] = new ResourceLocationBase("AddressablesMainContentCatalogCacheHash", catalog.InternalId, typeof(TextDataProvider).FullName);
                //Addressables.LoadContentCatalogAsync(catalog.InternalId);
                //LoadContentCatalogInternal(catalogs, 0, locMap);
            }

            IList<IResourceLocation> remoteCatalogList;
            if (!locMap.Locate("AddressablesMainContentCatalogRemoteHash", out remoteCatalogList))
            {
                Debug.LogError($"not found: {ResourceManagerRuntimeData.kCatalogAddress}");
            }
            else
            {
                Debug.Log($"catalogs.Count: {remoteCatalogList.Count}");
                var catalog = remoteCatalogList[0];
                Debug.Log($"internalId: {catalog.InternalId}");
                Debug.Log($"provideId: {catalog.ProviderId.ToString()}");
                dependencies[(int)ContentCatalogProvider.DependencyHashIndex.Remote] = new ResourceLocationBase("AddressablesMainContentCatalogRemoteHash", catalog.InternalId, typeof(TextDataProvider).FullName);

                //Addressables.LoadContentCatalogAsync(catalog.InternalId);
                //LoadContentCatalogInternal(catalogs, 0, locMap);
            }
            var location = new ResourceLocationBase("AddressablesMainContentCatalog", "Library/com.unity.addressables/StreamingAssetsCopy/aa/Windows/catalog.json", typeof(ContentCatalogProvider).FullName, dependencies);
            var tmp = Addressables.ResourceManager.ProvideResource<ContentCatalogData>(location);
            yield return tmp;
            Debug.Log($"tmp: {tmp.Result}");
        }

        //Debug.Log($"end reload catalog: {catalogHandle.Result}");
        //foreach(var provider in Addressables.ResourceManager.ResourceProviders)
        // {
        //     Debug.Log($"provider: {provider.ToString()}");
        // }
    }

    // Update is called once per frame
    void Update()
    {
    }
}
