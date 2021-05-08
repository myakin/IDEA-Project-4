using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventoryUIManager : MonoBehaviour {
    public static InventoryUIManager instance;

    private void Awake() {
        instance = this;
    }

    public Transform slotArea;
    public TextMeshProUGUI detailsTMP;
    public GameObject inventoryItemDataUnit;
    private int nextSlot;


    private void Start() {
        GenerateInventory();
    }

    public void GenerateInventory() {
        List<InventoryItem> inventoryList = InventoryManager.instance.GetInventoryList();
        for (int i=0; i<inventoryList.Count; i++) {
            Debug.Log(inventoryList[i].iconName);
            // her bir item'i sonraki bos slota ekle
            AddressablesManager.instance.LoadAnyTypeAddressable<Sprite> (
                inventoryList[i].iconName,
                delegate (Sprite loadedObject, string aReturningAddressableKey, int aReturningIndex) {
                    if (nextSlot<slotArea.childCount) {
                        // slotArea.GetChild(nextSlot).GetComponent<Image>().sprite = loadedObject;
                        GameObject obj = Instantiate(inventoryItemDataUnit, slotArea.GetChild(nextSlot));
                        // obj.transform.localPosition = Vector3.zero;
                        obj.GetComponent<InventoryItemDataUnitManager>().Initiate(inventoryList[aReturningIndex].title, inventoryList[aReturningIndex].amount, loadedObject, inventoryList[aReturningIndex].details);
                        obj.SetActive(true);
                        nextSlot++;
                    } else {
                        Debug.Log("No available inventory slots");
                    }

                    // AddressablesManager.instance.ReleaseAddressableLoadedByAnyTypeMethod(aReturningAddressableKey);
                },
                i
            ); 
        }
    }

    public void SetDetails(string aDetailsText) {
        detailsTMP.text = aDetailsText;
    }


}
