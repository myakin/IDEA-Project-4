using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour {
    public Transform raycastDummy;
    private bool byPass = true;
    private float bulletSpeed;
    private Vector3 initPos;
    private bool isInitPosSet;
    private float sqrRange;
    private float damage;

    public void Initiate(float aRange, float aBulletSpeed, float aDamage) {
        sqrRange = aRange * aRange;
        bulletSpeed = aBulletSpeed;
        damage = aDamage;
        byPass = false;
    }

    private void Update() {
        if (!byPass) {
            if (IsBulletInRange()) {
                MoveBullet();
                DetectCollision();
            } else {
                DestoryObject();
            }
        }
    }

    private void MoveBullet() {
        transform.position += transform.forward * (bulletSpeed * Time.deltaTime);
    }
    private void DetectCollision() {
        RaycastHit hit;
        if (Physics.Raycast(raycastDummy.position, raycastDummy.forward, out hit, 0.2f, 1<<0 | 1<<7, QueryTriggerInteraction.Ignore)) {
            Debug.Log(hit.collider.name);
            if (hit.collider.gameObject.GetComponent<Health>()) {
                hit.collider.gameObject.GetComponent<Health>().ModifyHealth(damage);
            }
            DestoryObject();
        }
    }
    private bool IsBulletInRange() {
        if (!isInitPosSet) {
            isInitPosSet = true;
            initPos = transform.position;
        }
        return (transform.position - initPos).sqrMagnitude < sqrRange;
    }
    private void DestoryObject() {
        Destroy(gameObject);
    }

}
