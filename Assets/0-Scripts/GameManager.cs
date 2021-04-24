using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {
    public static GameManager instance;

    private void Awake() {
        instance = this;
    }

    public string startSceneName;

    public void SpawnWorld() {
        AddressablesManager.instance.LoadAddressableSceneAdditive(
            startSceneName,
            delegate {
                // GameObject.FindGameObjectWithTag("Player").GetComponent<Rigidbody>().isKinematic = false;
                MultiplayerManager.instance.SpawnPlayer();
            }
        );
    }
}
