using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour {
    public Transform raycastDummy;
    public float bulletSpeed = 1;
    public float range = 200;
    private Vector3 initPos;
    private bool isInitPosSet;
    private float sqrMagnitude;

    private void Start() {
        sqrMagnitude = range * range;
    }

    private void Update() {
        if (IsBulletInRange()) {
            MoveBullet();
            DetectCollision();
        } else {
            DestoryObject();
        }
    }

    private void MoveBullet() {
        if (!isInitPosSet) {
            isInitPosSet = true;
            initPos = transform.position;
        }
        transform.position += transform.forward * (bulletSpeed * Time.deltaTime);
    }
    private void DetectCollision() {
        RaycastHit hit;
        if (Physics.Raycast(raycastDummy.position, raycastDummy.forward, out hit, 0.2f, 1<<0, QueryTriggerInteraction.Ignore)) {
            Debug.Log(hit.collider.name);
            DestoryObject();
            
        }
    }
    private bool IsBulletInRange() {
        return (transform.position - initPos).sqrMagnitude < sqrMagnitude;
    }
    private void DestoryObject() {
        Destroy(gameObject);
    }

}
