using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class PlayerCameraAssigner : MonoBehaviour {
    public Transform cameraPositionDummy;
    public bool useCinemachineCamera;
    private void Start() {
        if (GetComponent<PlayerController>().isPlayerInstance) {
            Transform cam = Camera.main.transform;
            if (!useCinemachineCamera) {
                cam.SetParent(cameraPositionDummy);
                cam.localPosition = Vector3.zero;
                cam.localRotation = Quaternion.identity;
                GetComponent<PlayerController>().playerCamera = cam;

                cam.GetComponent<CameraFollow>().followTarget = cameraPositionDummy;
                cam.GetComponent<CameraFollow>().byPass = false;
            } else {
                if (cam.GetComponent<CameraFollow>()) {
                    cam.GetComponent<CameraFollow>().enabled = false;
                }
                GameObject cinemachineCam = GameObject.FindGameObjectWithTag("CinemachineThirdPersonVCam");
                cinemachineCam.GetComponent<CinemachineVirtualCamera>().m_Follow = cameraPositionDummy;
            }

        }
        Destroy(this);
    }

    
}
