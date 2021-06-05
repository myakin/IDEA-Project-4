using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ParticleSystemShot))]
public class ParticleSystemShotEditor : Editor {
    public override void OnInspectorGUI() {
        DrawDefaultInspector();

        ParticleSystemShot script = (ParticleSystemShot)target;

        if (GUILayout.Button("Make Shot")) {
            script.Fire();
        }

        if (GUILayout.Button("Clear Pool")) {
            script.ClearPool();
        }

    }
}
