using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {
    public string startSceneName;

    private void Start() {
        AddressablesManager.instance.LoadAddressableSceneAdditive(
            startSceneName,
            delegate {
                GameObject.FindGameObjectWithTag("Player").GetComponent<Rigidbody>().isKinematic = false;
            }
        );
    }
}
