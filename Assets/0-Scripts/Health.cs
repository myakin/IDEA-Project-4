using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour {
    public HUDCanvasManager HUDCanvasManager;
    public float currentHealth = 100;
    public float maxHealth = 100;
    public bool isDead;


    public void ModifyHealth(float aChangeRate) {
        currentHealth+=aChangeRate;
        if (currentHealth==0) {
            isDead = true;
        }
        if (currentHealth>maxHealth) {
            currentHealth = maxHealth;
        } else if (currentHealth<0) {
            currentHealth = 0;
        }
        if (GetComponent<PlayerController>().isPlayerInstance) {
            UIManager.instance.SetHealthProgressbar(currentHealth/maxHealth);
        } else {
            HUDCanvasManager.ModifyHealth(currentHealth/maxHealth);
        }
    }
    
}
