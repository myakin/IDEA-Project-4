using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectLoader : MonoBehaviour {
    public string[] prefabName;
    public List<GameObject> loadedAssets = new List<GameObject>();

    public void LoadAssets() {
        Debug.Log("Loading Assets");
        for (int i=0; i<prefabName.Length; i++) {
            GameObject obj = Instantiate(Resources.Load(prefabName[i]) as GameObject);
            loadedAssets.Add(obj);
        }
    }

    public void UnloadAssets() {
        for (int i=0; i<loadedAssets.Count; i++) {
            Destroy(loadedAssets[i]);
        }
    }
}
