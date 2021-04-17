using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddressableObjectOnDestroy : MonoBehaviour {
    public string addressableKey;

    private void OnDestroy() {
        AddressablesManager.instance.ReleaseAddressableObject(addressableKey);
    }
}
