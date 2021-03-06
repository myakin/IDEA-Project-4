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


    private Queue<SpawnRequest> requests = new Queue<SpawnRequest>();
    private Dictionary<string, GameObject> keysAndObjectsInMemory = new Dictionary<string, GameObject>();
    private Dictionary<string, AsyncOperationHandle> keysAndOpHandles = new Dictionary<string, AsyncOperationHandle>();
    private Dictionary<string, int> keysAndObjectCounts = new Dictionary<string, int>();
    public delegate void OnSpawnObject(GameObject anObject);

    struct SpawnRequest {
        public string addressableKey;
        public Vector3 spawnPosition;
        public Quaternion spawnRotation;
        public Transform parent;
        public OnSpawnObject onSpawn;

        public SpawnRequest(string addressableKey, Vector3 spawnPosition, Quaternion spawnRotation, Transform aParent, OnSpawnObject onSpawn) {
            this.addressableKey = addressableKey;
            this.spawnPosition = spawnPosition;
            this.spawnRotation = spawnRotation;
            this.parent = aParent;
            this.onSpawn = onSpawn;
        }
    }

    private IEnumerator addressableObjectLoadingCoroutine;

    
    public void SpawnObject(string addressableKey, Vector3 spawnPosition, Quaternion spawnRotation, Transform aParent, OnSpawnObject onSpawn)
    {
        // requests.Enqueue(new SpawnRequest(addressableKey, spawnPosition, spawnRotation, aParent, onSpawn));
        // if (addressableObjectLoadingCoroutine==null)
        // {
        //     addressableObjectLoadingCoroutine = AddressableObjectLoadingCoroutine(addressableKey, spawnPosition, spawnRotation, aParent, onSpawn);
        //     StartCoroutine(addressableObjectLoadingCoroutine);
        // }

        if (keysAndObjectsInMemory.ContainsKey(addressableKey)) {
            // Debug.Log("Getting from pre-loaded sample");
            GameObject obj = Instantiate(keysAndObjectsInMemory[addressableKey], spawnPosition, spawnRotation, aParent);
            obj.AddComponent<AddressableObjectOnDestroy>().addressableKey = addressableKey;
            keysAndObjectCounts[addressableKey]++;
            onSpawn(obj);
        } else {
            StartCoroutine(AddressableObjectLoadingCoroutine(addressableKey, spawnPosition, spawnRotation, aParent, onSpawn));
        }
    }
    private IEnumerator AddressableObjectLoadingCoroutine(string addressableKey, Vector3 spawnPosition, Quaternion spawnRotation, Transform aParent, OnSpawnObject onSpawn) {
        // while (requests.Count > 0)
        // {
        //     if (keysAndObjectsInMemory.ContainsKey(addressableKey))
        //     {
        //         SpawnRequest request = requests.Dequeue();
        //         GameObject obj = Instantiate(keysAndObjectsInMemory[request.addressableKey], request.spawnPosition, request.spawnRotation, request.parent);
        //         obj.AddComponent<AddressableObjectOnDestroy>().addressableKey = addressableKey;
        //         keysAndObjectCounts[addressableKey]++;
                
        //         request.onSpawn(obj);
        //     }
        //     else
        //     {
        //         SpawnRequest request = requests.Dequeue();
        //         AsyncOperationHandle<GameObject> op = Addressables.LoadAssetAsync<GameObject>(request.addressableKey);
        //         yield return op;

        //         if (op.Status == AsyncOperationStatus.Succeeded)
        //         {
        //             GameObject objectInMemory = op.Result;
        //             keysAndObjectsInMemory.Add(request.addressableKey, objectInMemory);
        //             keysAndObjectCounts.Add(request.addressableKey, 1);

        //             GameObject obj = Instantiate(objectInMemory, request.spawnPosition, request.spawnRotation, request.parent);
        //             obj.AddComponent<AddressableObjectOnDestroy>().addressableKey = addressableKey;
                    
        //             keysAndOpHandles.Add(request.addressableKey, op);

        //             request.onSpawn(obj);

        //         }
        //     }
        // }
        // addressableObjectLoadingCoroutine = null;

        // Debug.Log("Getting from archieve");
        string key = addressableKey;
        Vector3 sp = spawnPosition;
        Quaternion sr = spawnRotation;
        Transform p = aParent;
        OnSpawnObject os = onSpawn;

        AsyncOperationHandle<GameObject> op = Addressables.LoadAssetAsync<GameObject>(key);
        yield return op;

        if (op.Status == AsyncOperationStatus.Succeeded) {
            GameObject objectInMemory = op.Result;
            if (!keysAndObjectsInMemory.ContainsKey(key)){
                keysAndObjectsInMemory.Add(key, objectInMemory);
                keysAndObjectCounts.Add(key, 1);
            } else {
                keysAndObjectCounts[key]++;
            }

            GameObject obj = Instantiate(objectInMemory, sp, sr, p);
            obj.AddComponent<AddressableObjectOnDestroy>().addressableKey = key;

            os(obj);
            
            if (!keysAndOpHandles.ContainsKey(key))
                keysAndOpHandles.Add(key, op);

        }
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





    public Dictionary<string, AsyncOperationHandle> recordOfUsedHandles = new Dictionary<string, AsyncOperationHandle>();
    public Dictionary<AsyncOperationHandle, int> countOfHandleUses = new Dictionary<AsyncOperationHandle, int>();
    public delegate void OnObjectLoaded<T>(T loadedObject, string addressableKey, int returningIndex);
    public void LoadAnyTypeAddressable<T>(string key, OnObjectLoaded<T> onObjectLoaded, int anIndex = 0) {
        if (recordOfUsedHandles.ContainsKey(key)) {
            T obj = (T)recordOfUsedHandles[key].Result;
            onObjectLoaded(obj, key, anIndex);
        } else {
            StartCoroutine(LoadAnyTypeAddressableCoroutine(key, onObjectLoaded, anIndex));
        }
    }

    private IEnumerator LoadAnyTypeAddressableCoroutine<T>(string key, OnObjectLoaded<T> onObjectLoaded, int anIndex = 0) {
        AsyncOperationHandle<T> op = Addressables.LoadAssetAsync<T>(key);
        yield return op;
        if (op.Status == AsyncOperationStatus.Succeeded) {
            T obj = op.Result;

            if (!recordOfUsedHandles.ContainsKey(key)) {
                recordOfUsedHandles.Add(key, op);
            }
            if (!countOfHandleUses.ContainsKey(op)) {
                countOfHandleUses.Add(op, 0);
            }
            countOfHandleUses[op]++;

            onObjectLoaded(obj, key, anIndex);
        }
    }

    public void ReleaseAddressableLoadedByAnyTypeMethod(string key) {
        if (recordOfUsedHandles.ContainsKey(key)) {
            AsyncOperationHandle op = recordOfUsedHandles[key];
            countOfHandleUses[op]--;
            if (countOfHandleUses[op]==0) {
                Addressables.Release(op);
                recordOfUsedHandles.Remove(key);
                countOfHandleUses.Remove(op);
            }
        } else {
            Debug.LogWarning("ReleaseAddressableLoadedByAnyTypeMethod(string key) received a key that does not exist in the system.");
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
