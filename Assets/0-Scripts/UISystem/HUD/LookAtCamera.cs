using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtCamera : MonoBehaviour {
    private Transform mainCam;

    private void Start() {
        mainCam = Camera.main.transform;
    }

    void Update() {
        transform.rotation = Quaternion.LookRotation (transform.position- mainCam.position, transform.up);
    }
}
