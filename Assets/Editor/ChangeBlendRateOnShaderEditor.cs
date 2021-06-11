using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ChangeBlendRateOnShader))]
public class ChangeBlendRateOnShaderEditor : Editor {
    
    public override void OnInspectorGUI() {
        DrawDefaultInspector();

        ChangeBlendRateOnShader script = (ChangeBlendRateOnShader)target;

        if (GUILayout.Button("Increase Blend")) {
            script.IncreaseBlend(0.1f);
        }
        if (GUILayout.Button("Decrease Blend")) {
            script.DecreaseBlend(0.1f);
        }

    }
}
