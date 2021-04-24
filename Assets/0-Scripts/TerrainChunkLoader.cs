using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TerrainChunkLoader : MonoBehaviour {
    public UnityEvent OnEnteryVectorInSameDirectionWithMyForward;
    public UnityEvent OnEnteryVectorInOppositeDirectionWithMyForward;
    public UnityEvent OnExitVectorInSameDirectionWithMyForward;
    public UnityEvent OnExitVectorInOppositeDirectionWithMyForward;
    
    private void OnTriggerEnter(Collider other) {
        if (other.tag=="Player") {
            if (IsEntryVectorInSameDirectionWithMyForward(other.transform)) {
                // if (OnEnteryVectorInSameDirectionWithMyForward!=null) {
                //     OnEnteryVectorInSameDirectionWithMyForward.Invoke();
                // }
                OnEnteryVectorInSameDirectionWithMyForward?.Invoke();
            } else {
                OnEnteryVectorInOppositeDirectionWithMyForward?.Invoke();
            }
        }
    }

    private void OnTriggerExit(Collider other) {
        if (other.tag=="Player") {
            if (IsEntryVectorInSameDirectionWithMyForward(other.transform)) {
                OnExitVectorInSameDirectionWithMyForward?.Invoke();
            } else {
                OnExitVectorInOppositeDirectionWithMyForward?.Invoke();
            }
        }
    }

    private bool IsEntryVectorInSameDirectionWithMyForward(Transform player) {
        Vector3 entryVector = player.position - transform.position;
        // if (Vector3.Angle(entryVector, transform.forward) > 90) {
        //     return false;
        // }
        // return true;
        if (Vector3.Dot(entryVector.normalized, transform.forward)>0) {
            return true;
        }
        return false;
    }

}
