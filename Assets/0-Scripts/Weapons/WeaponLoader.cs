using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponLoader : MonoBehaviour {
    public Transform weaponRestPositionDummy;

    // private void Start() {
    //     LoadLastWeapon();
    // }

    public void LoadLastWeapon(string aWeaponName = "") {
        string weaponToLoad = aWeaponName;
        if (string.IsNullOrEmpty(weaponToLoad)) {
            weaponToLoad = "G28 Rifle_PaintBall"; // default weapon
        }
        AddressablesManager.instance.SpawnObject(
            weaponToLoad,
            weaponRestPositionDummy.position,
            weaponRestPositionDummy.rotation,
            weaponRestPositionDummy,
            delegate (GameObject obj) {
                GetComponent<PlayerController>().weaponManager = obj.GetComponent<WeaponManager>();
                obj.transform.SetParent(weaponRestPositionDummy);
                obj.transform.localPosition = Vector3.zero;
                obj.transform.localRotation = Quaternion.identity;
            }
        );
    }
}
