using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour {
    public Transform followTarget;
    public float followDistance = 3;
    public float followUpOffset = 0.5f;
    public float sensitivity = 1f;
    public float dampening = 0.1f;
    public bool byPass = true;

    private float mouseY;
    private Vector3 oldPosition;
    
    void Update() {
        if (!byPass) {
            if (transform.parent!=null) {
                transform.SetParent(null);
            }
            
            transform.position = Vector3.LerpUnclamped(oldPosition, followTarget.position + (-followTarget.forward * followDistance) + (followTarget.up * followUpOffset), dampening);
            mouseY += Input.GetAxis("Mouse Y") * sensitivity;
            mouseY = Mathf.Clamp(mouseY, -90f, 90f);
            transform.rotation = Quaternion.LookRotation(followTarget.forward, followTarget.up) * Quaternion.Euler(-mouseY, 0f, 0f);

            oldPosition = transform.position;
            
        }
    }
}
