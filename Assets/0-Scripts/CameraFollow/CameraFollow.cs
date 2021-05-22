using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour {
    public Transform followTarget;
    public float followDistance = 3;
    public float followUpOffset = 0.5f;
    public float rightOffset = 0;
    public float sensitivity = 1f;
    public float dampening = 0.1f;
    public float collisionSphereRadius = 0.5f;
    public bool useCalculationStyleCorrection;
    public bool byPass = true;

    public bool shakeCam;

    private float mouseY;
    private Vector3 oldPosition;
    private Vector3 backOffset;
    private bool isCorrectingCamera, isCameraShakeActive;

    private void Update() {
        if (shakeCam) {
            shakeCam = false;
            ShakeCamera(5, 30);
        }
    }

    void LateUpdate() {
        if (!byPass) {
            if (transform.parent!=null) {
                transform.SetParent(null);
            }
           
            CheckObjectsBetweenCameraAndTarget();

            if (!useCalculationStyleCorrection) {
                if (!isCorrectingCamera) {
                    transform.position = Vector3.LerpUnclamped(oldPosition, followTarget.position + (-followTarget.forward * followDistance) + (followTarget.up * followUpOffset) + (followTarget.right * rightOffset), dampening);
                }
            } else {
                transform.position = Vector3.LerpUnclamped(oldPosition, followTarget.position + backOffset + followTarget.up * followUpOffset + (followTarget.right * rightOffset), dampening);
            }
            
            mouseY += Input.GetAxis("Mouse Y") * sensitivity;
            mouseY = Mathf.Clamp(mouseY, -90f, 90f);

            // if (!isCameraShakeActive) {
            transform.rotation = Quaternion.LookRotation(followTarget.forward, followTarget.up) * Quaternion.Euler(-mouseY, 0f, 0f);
            // }

            oldPosition = transform.position;
            
        }
    }

   

    private void CheckObjectsBetweenCameraAndTarget() {
        RaycastHit hit;
        if (Physics.SphereCast(followTarget.position, collisionSphereRadius, -followTarget.forward, out hit, followDistance, 1<<0, QueryTriggerInteraction.Ignore)) {
            // Debug.Log(hit.collider.name);
            if (!useCalculationStyleCorrection) {
                isCorrectingCamera=true;
                transform.position = Vector3.LerpUnclamped(transform.position, hit.point + (followTarget.up * followUpOffset), 0.1f);
            
            } else {   
                Vector3 collisionOffset = hit.point - transform.position;
                // float collisionAngleForward = Vector3.SignedAngle(transform.forward, collisionOffset, transform.up);
                float collisionAngle = Vector3.Angle(-followTarget.forward, collisionOffset);
                float collisionDistance = hit.distance;
                float backOffsetScalarValue = Mathf.Cos(collisionAngle) * collisionDistance;
                backOffset = -followTarget.forward * backOffsetScalarValue;
            }

        } else {
            if (!useCalculationStyleCorrection) {
                isCorrectingCamera = false;
            } else {
                backOffset = (-followTarget.forward * followDistance);
            }
        }
    }

    private IEnumerator shakeCoroutine;
    private void ShakeCamera(float shakeTime, float shakeStrength) {
        if (shakeCoroutine == null) {
            shakeCoroutine = ShakeCoroutine(shakeTime, shakeStrength);
            StartCoroutine(shakeCoroutine);
        }
    }
    private IEnumerator ShakeCoroutine(float shakeTime, float shakeStrength) {
        float timer= 0;
        Quaternion camInitialRot = transform.rotation;
        Quaternion shakeOffset = Quaternion.identity;
        float totalShakeValue = 0;
        float threshold = shakeStrength;
        float multiplier = 1;
        while (timer<shakeTime) {
            if (totalShakeValue>threshold) {
                multiplier*=-1;
            }
            totalShakeValue+= (1f * multiplier);
        
            shakeOffset*=Quaternion.Euler(0, totalShakeValue, 0);
            transform.rotation *= shakeOffset;

            timer+=Time.deltaTime;
            yield return null;
        }
        transform.rotation = camInitialRot;
        shakeCoroutine = null;
    }


}
