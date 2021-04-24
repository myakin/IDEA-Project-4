using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponManager : MonoBehaviour {
    public string bulletPrefabName;
    public Transform bulletGenerationPointDummy;

    private bool isPermittedToGenerateBullet;

    public void Fire() {
        if (isPermittedToGenerateBullet) {
            AddressablesManager.instance.SpawnObject(
                bulletPrefabName, 
                Vector3.zero, 
                Quaternion.identity, 
                null, 
                delegate(GameObject anobject) { 
                    isPermittedToGenerateBullet=false;
                }
            ); 
        } 
    }

    public void ReleaseFiringConstrains() {
        isPermittedToGenerateBullet = true;
    }

}
