using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class ParticleSystemShot : MonoBehaviour {
    public ParticleSystem ps;
    public Color sColor;
    public GameObject sparklePrefab, defEffectPrefab;
    private Queue<GameObject> sparklePool = new Queue<GameObject>();
    private Queue<GameObject> defPool = new Queue<GameObject>();
    private List<ParticleCollisionEvent> collisionEvents= new List<ParticleCollisionEvent>();
    private float damage;

    // private void Start() {
    //     mainModule = ps.main;
    // }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.U)) {
            ps.Emit(1);
        }
    }

    public void Fire(float damageRate = 0) {
        ParticleSystem.MainModule mainModule = ps.main;
        sColor = new Color(Random.Range(0,1f), Random.Range(0,1f), Random.Range(0,1f), 1f);
        mainModule.startColor = sColor;
        damage = damageRate;
        ps.Emit(1);
    }


    private void OnParticleCollision(GameObject other) {
        Debug.Log(other.name);

        int numCollisionEvents = ps.GetCollisionEvents(other, collisionEvents);

        int i=0;
        while (i<numCollisionEvents) {
            Vector3 normal = collisionEvents[i].normal;
            Vector3 pos = collisionEvents[i].intersection;
            InitiateSparkleEffect(pos, normal);
            InitiateDefEffect(pos, normal);

            if (other.GetComponent<Health>()) {
                other.GetComponent<Health>().ModifyHealth(damage);
            }
            i++;
        }
    }

    private void InitiateSparkleEffect(Vector3 aPosition, Vector3 aNormal) {
        if (sparklePool.Count>0) {
            GameObject obj = sparklePool.Dequeue();
            obj.transform.position = aPosition;
            obj.transform.rotation = Quaternion.LookRotation(aNormal);
            obj.transform.SetParent(null);
            // obj.SetActive(true);
            // obj.GetComponent<ParticleSystem>().Emit(40);
            obj.GetComponent<ParticleSystem>().Play();

            StartCoroutine(ReturnToPool(obj));

        } else {
            GameObject obj = Instantiate(sparklePrefab, aPosition, Quaternion.LookRotation(aNormal), null);
            // obj.GetComponent<ParticleSystem>().Play();
            StartCoroutine(ReturnToPool(obj));

            // obj.GetComponent<ParticleSystem>().Emit(40);
            obj.GetComponent<ParticleSystem>().Play();

        }
    }
    private void InitiateDefEffect(Vector3 aPosition, Vector3 aNormal) {
        if (sparklePool.Count>0) {
            GameObject obj = defPool.Dequeue();
            obj.transform.position = aPosition;
            obj.transform.rotation = Quaternion.LookRotation(-aNormal);
            obj.transform.SetParent(null);
            // obj.SetActive(true);

            // ParticleSystem.MainModule mainModuleD = obj.GetComponent<ParticleSystem>().main;
            // mainModuleD.startColor = sColor;
            // obj.GetComponent<ParticleSystem>().Emit(40);
            obj.GetComponent<ParticleSystem>().Play();

            StartCoroutine(ReturnToPoolDefEffect(obj));

        } else {
            GameObject obj = Instantiate(defEffectPrefab, aPosition, Quaternion.LookRotation(-aNormal), null);
            // obj.GetComponent<ParticleSystem>().Play();
            StartCoroutine(ReturnToPoolDefEffect(obj));

            // ParticleSystem.MainModule mainModuleD = obj.GetComponent<ParticleSystem>().main;
            // mainModuleD.startColor = sColor;

            // obj.GetComponent<ParticleSystem>().Emit(40);
            obj.GetComponent<ParticleSystem>().Play();

        }
    }

    private IEnumerator ReturnToPool(GameObject anObj) {
        yield return new WaitForSeconds(1f);
        // anObj.SetActive(false);
        sparklePool.Enqueue(anObj);
    }
    private IEnumerator ReturnToPoolDefEffect(GameObject anObj) {
        yield return new WaitForSeconds(1f);
        // anObj.SetActive(false);
        defPool.Enqueue(anObj);
    }

    public void ClearPool() {
        while (sparklePool.Count>0) {
            DestroyImmediate(sparklePool.Dequeue());
        }
        sparklePool.Clear();
    }
}
