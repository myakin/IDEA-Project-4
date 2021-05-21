using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIController : MonoBehaviour {
    
    
    void Update() {
        if (Input.GetKeyDown(KeyCode.I)) {
            if (InventoryUIManager.instance==null) {
                AddressablesManager.instance.LoadAddressableSceneAdditive("InventoryUI");
            } else {
                AddressablesManager.instance.UnloadAddressableScene("InventoryUI");
            }
        }    
    }
}
