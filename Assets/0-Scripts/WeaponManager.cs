using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponManager : MonoBehaviour {
    public string bulletPrefabName;
    public Transform bulletGenerationPointDummy;
    public float fireRate = 0.5f;

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
