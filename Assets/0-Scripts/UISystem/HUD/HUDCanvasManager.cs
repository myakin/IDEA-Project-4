using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUDCanvasManager : MonoBehaviour {
    public Image healthProgressbar;


    public void ModifyHealth(float aRate) {
        healthProgressbar.fillAmount = aRate;
    }
}
