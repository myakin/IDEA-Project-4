using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoCancelParenting : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        transform.SetParent(AddressablesManager.instance.transform);
        transform.SetParent(null);    
    }

    
}
