using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCameraAssigner : MonoBehaviour {
    public Transform cameraPositionDummy;
    private void Start() {
        if (GetComponent<PlayerController>().isPlayerInstance) {
            Transform cam = Camera.main.transform;
            cam.SetParent(cameraPositionDummy);
            cam.localPosition = Vector3.zero;
            cam.localRotation = Quaternion.identity;

        }
        Destroy(gameObject);
    }

    
}
