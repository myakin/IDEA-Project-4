using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour {
    public static InventoryManager instance;

    private void Awake() {
        if (instance==null) {
            instance = this;
            DontDestroyOnLoad(gameObject);
        } else {
            if (instance!=this) {
                Destroy(gameObject);
            }
        }
    }

    public List<InventoryItem> inventory = new List<InventoryItem>();

    public List<InventoryItem> GetInventoryList() {
        return inventory;
    }

}
