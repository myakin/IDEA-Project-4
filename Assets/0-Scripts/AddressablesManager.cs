using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.SceneManagement;

public class AddressablesManager : MonoBehaviour {
    public static AddressablesManager instance;

    private void Awake()
    {
        if(instance==null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        } else
        {
            if (instance!=this)
            {
                Destroy(gameObject);
            }
        }
    }


    private Queue<string> requests = new Queue<string>();
    private Dictionary<string, GameObject> keysAndObjectsInMemory = new Dictionary<string, GameObject>();
    private Dictionary<string, AsyncOperationHandle> keysAndOpHandles = new Dictionary<string, AsyncOperationHandle>();
    private Dictionary<string, int> keysAndObjectCounts = new Dictionary<string, int>();
    public delegate void OnSpawnObject(GameObject anObject);


    private IEnumerator addressableObjectLoadingCoroutine;

    public void SpawnObject(string addressableKey, Vector3 spawnPosition, Quaternion spawnRotation, Transform aParent, OnSpawnObject onSpawn)
    {
        requests.Enqueue(addressableKey);

        if (addressableObjectLoadingCoroutine==null)
        {
            addressableObjectLoadingCoroutine = AddressableObjectLoadingCoroutine(addressableKey, spawnPosition, spawnRotation, aParent, onSpawn);
            StartCoroutine(addressableObjectLoadingCoroutine);
        }
       

    }
    private IEnumerator AddressableObjectLoadingCoroutine(string addressableKey, Vector3 spawnPosition, Quaternion spawnRotation, Transform aParent, OnSpawnObject onSpawn)
    {
        while (requests.Count > 0)
        {
            if (keysAndObjectsInMemory.ContainsKey(addressableKey))
            {
                GameObject obj = Instantiate(keysAndObjectsInMemory[addressableKey], spawnPosition, spawnRotation, aParent);
                obj.AddComponent<AddressableObjectOnDestroy>().addressableKey = addressableKey;
                keysAndObjectCounts[addressableKey]++;
                requests.Dequeue();
            }
            else
            {
                AsyncOperationHandle<GameObject> op = Addressables.LoadAssetAsync<GameObject>(addressableKey);
                yield return op;

                if (op.Status == AsyncOperationStatus.Succeeded)
                {
                    GameObject objectInMemory = op.Result;
                    keysAndObjectsInMemory.Add(addressableKey, objectInMemory);
                    keysAndObjectCounts.Add(addressableKey, 1);

                    GameObject obj = Instantiate(objectInMemory, spawnPosition, spawnRotation, aParent);
                    obj.AddComponent<AddressableObjectOnDestroy>().addressableKey = addressableKey;
                    
                    keysAndOpHandles.Add(addressableKey, op);

                    requests.Dequeue();

                    onSpawn(obj);

                }
            }
        }
        addressableObjectLoadingCoroutine = null;
    }

    public void ReleaseAddressableObject(string addressableKey)
    {
        if (keysAndOpHandles.ContainsKey(addressableKey)) {
            keysAndObjectCounts[addressableKey]--;
            if (keysAndObjectCounts[addressableKey] == 0)
            {
                Addressables.Release(keysAndOpHandles[addressableKey]);
                keysAndObjectsInMemory.Remove(addressableKey);
                keysAndOpHandles.Remove(addressableKey);
                keysAndObjectCounts.Remove(addressableKey);

            }
        }
    }


    public delegate void PostSceneOperation();
    private List<string> loadingScenes = new List<string>();
    private List<string> loadedScenes = new List<string>();
    private Dictionary<string, AsyncOperationHandle> loadedSceneHandles = new Dictionary<string, AsyncOperationHandle>();

    public void LoadAddressableSceneAdditive(string key, PostSceneOperation onLoad = null, bool activateOnLoad = true) {
        if (loadedScenes.Contains(key) || loadingScenes.Contains(key)) {
            return;
        }
        loadingScenes.Add(key);

        // solution 1
        var op = Addressables.LoadSceneAsync (key, LoadSceneMode.Additive, activateOnLoad);
        op.Completed += operation => {
            // scene is already loaded, do some security registerations here
            loadedScenes.Add(key);
            loadingScenes.Remove(key);
            loadedSceneHandles.Add(key, op);
            if (onLoad!=null)
                onLoad();
        };

        // solution 2
        // StartCoroutine(AdditiveSceneLoading(key, onLoad, activateOnLoad));
    }
    // private IEnumerator AdditiveSceneLoading(string key, PostSceneOperation onLoad, bool activateOnLoad = true) {
    //     var op = AddressablesManager.LoadSceneAsync (key, LoadSceneMode.Additive, activateOnLoad);
    //     while (!op.done) {
    //         yield return null;
    //     }
    //     loadedScenes.Add(key);
    //     loadingScenes.Remove(key);

    //     onLoad();
    // }

    public void UnloadAddressableScene(string key, PostSceneOperation onUnload = null) {
        if (loadedScenes.Contains(key)) {  
            // TODO: run coroutine
            StartCoroutine(UnloadSceneCoroutine(key, onUnload));
            
        }
    }

    private IEnumerator UnloadSceneCoroutine(string key, PostSceneOperation onUnload = null){
        var op = Addressables.UnloadSceneAsync(loadedSceneHandles[key]);
        while (!op.IsDone) {
            yield return null;
        }
        loadedScenes.Remove(key);
        loadedSceneHandles.Remove(key);

        if (onUnload!=null) {
            onUnload();
        }
    }
   
}
