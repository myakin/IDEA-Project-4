using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventoryItemDataUnitManager : MonoBehaviour {
    public Button itemButton;
    public TextMeshProUGUI titleTMP, amountTMP;
    public Image iconImage;
    private string details;

    public void Initiate(string aTitle, int anAmount, Sprite anIcon, string aDetailsText) {
        titleTMP.text = aTitle;
        amountTMP.text = anAmount.ToString();
        iconImage.sprite = anIcon;
        details = aDetailsText;
        itemButton.onClick.AddListener(OnItemClicked);
    }

    private void OnItemClicked() {
        InventoryUIManager.instance.SetDetails(details);
    }

}
