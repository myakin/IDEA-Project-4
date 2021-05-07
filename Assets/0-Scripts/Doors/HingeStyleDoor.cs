using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HingeStyleDoor : MonoBehaviour {
    public Transform door, openRotDummy, ClosedRotDummy;
    private IEnumerator animationCoroutine;

    private void OnTriggerEnter(Collider other) {
        AnimateDoor(true);
    }

    private void OnTriggerExit(Collider other) {
        AnimateDoor(false);
    }

    private void AnimateDoor(bool shouldOpen) {
        if (animationCoroutine!=null) {
            StopCoroutine(animationCoroutine);
            animationCoroutine=null;
        }
        if (animationCoroutine==null) {
            animationCoroutine = DoorAnimationCoroutine(shouldOpen);
            StartCoroutine(animationCoroutine);
        }
    }
    private IEnumerator DoorAnimationCoroutine(bool shouldOpen) {
        Quaternion initialRot = door.localRotation;
        Quaternion targetRot = shouldOpen ? openRotDummy.localRotation : ClosedRotDummy.localRotation;

        float timer = 0;
        float duration = 1;
        while (timer<duration) {
            door.localRotation=Quaternion.Slerp(initialRot, targetRot, timer/duration);
            timer+=Time.deltaTime;
            yield return null;
        }
        door.localRotation = targetRot;

        animationCoroutine = null;
    }


}
