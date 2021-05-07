using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour {
    public static UIManager instance;

    private void Awake() {
        instance = this;
    }

    public GameObject connectingLabel, connectedLabel;
    public Image healthProgressbar,staminaProgressbar, thirstProgressbar, hungerPorgressbar, sleepProgressbar;


    public void SetStatus(bool isConnectedToServer) {
        if (isConnectedToServer) {
            connectingLabel.SetActive(false);
            connectedLabel.SetActive(true);
        } else {
            connectingLabel.SetActive(true);
            connectedLabel.SetActive(false);
        }
    }

    public void SetHealthProgressbar(float aRate) {
        healthProgressbar.fillAmount = aRate;
    }
}
