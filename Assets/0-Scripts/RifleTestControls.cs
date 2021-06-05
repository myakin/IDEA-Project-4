using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RifleTestControls : MonoBehaviour {
    
    void Update() {
        float ver = Input.GetAxis("Vertical");
        float hor = Input.GetAxis("Horizontal");

        transform.rotation *= Quaternion.Euler(-ver * 0.2f, hor * 0.2f, 0);
    }
}
