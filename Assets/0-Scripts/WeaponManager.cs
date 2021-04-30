using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponManager : MonoBehaviour {
    public string bulletPrefabName;
    public Transform bulletGenerationPointDummy;
    public float fireRate = 0.5f;
    public float range = 100;
    public float bulletSpeed = 1;
    public float maxDamage = 1.5f;
    

    private bool isPermittedToGenerateBullet = true;

    public void Fire() {
        if (isPermittedToGenerateBullet) {
            isPermittedToGenerateBullet = false;
            AddressablesManager.instance.SpawnObject(
                bulletPrefabName, 
                bulletGenerationPointDummy.position, 
                bulletGenerationPointDummy.rotation, 
                null, 
                delegate(GameObject anobject) { 
                    anobject.GetComponent<Bullet>().Initiate(range, bulletSpeed, -Random.Range(0.3f, maxDamage));
                    ReleaseFiringConstrains();
                }
            ); 
        } 
    }

    private IEnumerator fireHoldCoroutine;
    public void ReleaseFiringConstrains() {
        if (fireHoldCoroutine==null) {
            fireHoldCoroutine = HoldFire();
            StartCoroutine(fireHoldCoroutine);
        }
    }

    private IEnumerator HoldFire() {
        yield return new WaitForSeconds(1/fireRate);
        isPermittedToGenerateBullet = true;
        fireHoldCoroutine = null;
    }

}
