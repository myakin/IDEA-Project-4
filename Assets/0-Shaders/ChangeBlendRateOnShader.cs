using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeBlendRateOnShader : MonoBehaviour {

    public void IncreaseBlend(float aRate) {
        float value = GetComponent<MeshRenderer>().materials[0].GetFloat("_BlendRate");
        value+=aRate;
        if (value>1) {
            value = 1;
        }
        GetComponent<MeshRenderer>().materials[0].SetFloat("_BlendRate", value);
    }
    public void DecreaseBlend(float aRate) {
        float value = GetComponent<MeshRenderer>().materials[0].GetFloat("_BlendRate");
        value-=aRate;
        if (value<0) {
            value = 0;
        }
        GetComponent<MeshRenderer>().materials[0].SetFloat("_BlendRate", value);
    }
}
