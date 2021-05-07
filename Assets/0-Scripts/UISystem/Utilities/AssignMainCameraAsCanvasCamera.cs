using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AssignMainCameraAsCanvasCamera : MonoBehaviour {
    void Start() {
        Camera cam = Camera.main;
        GetComponent<Canvas>().worldCamera = cam;  
        Destroy(this);  
    }
}
