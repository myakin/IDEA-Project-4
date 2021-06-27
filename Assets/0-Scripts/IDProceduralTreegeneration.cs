using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;
// using Unity.Rendering;

public class IDProceduralTreegeneration : MonoBehaviour {
    public GameObject treePrefab;
    public int amount = 1000;

    public Mesh prismMesh;
    public Material prismMaterial;

    private float startTime, endTime;
    private bool isGenerationStarted;
    public EntityManager _entityManager;

    private void Update() {
        if (Input.GetKeyDown(KeyCode.G) && !isGenerationStarted) {
            isGenerationStarted = true;
            startTime = Time.realtimeSinceStartup;

            GenerateTreesFast();
            // GenerateTrees();

            endTime = Time.realtimeSinceStartup;
            Debug.Log("Finished in "+(endTime - startTime)+" seconds");
            
        }
    }

    private void GenerateTrees() {
        for (int i=0; i<amount; i++) {
            Vector3 locPos = new Vector3( UnityEngine.Random.Range(-250, 250), 0, UnityEngine.Random.Range(-250,250));

            GameObject obj = Instantiate(treePrefab, transform);
            // obj.transform.localPosition = localPosition;
            obj.transform.position = GetSurfaceWorldPosition(transform.TransformPoint(locPos));

            if (obj.transform.position.y<-50) {
                obj.SetActive(false);
            } else {
                obj.transform.rotation = Quaternion.Euler(0, UnityEngine.Random.Range(0, 360), 0);
                float scale = UnityEngine.Random.Range(0.6f, 1.4f);
                obj.transform.localScale = Vector3.one * scale;
            }
        }
    }
    private void GenerateTreesFast() {
        Vector3[] origins = new Vector3[amount];
        
        // for (int i=0; i<amount; i++) {
        //     origins[i] = transform.TransformPoint(new Vector3( UnityEngine.Random.Range(-250, 250), 0, UnityEngine.Random.Range(-250,250)));
        // }
        // RaycastHit[] hits = PerformMultiRaycast(origins);
        // for (int i=0; i<hits.Length; i++) {
        //     if (hits[i].collider.name != "NoFoliageCollider") {
        //         Quaternion rot = Quaternion.Euler(0, UnityEngine.Random.Range(0, 360), 0);
        //         float scale = UnityEngine.Random.Range(0.6f, 1.4f);
        //         Vector3 locScale = Vector3.one * scale;
        //         GameObject obj = Instantiate(treePrefab, hits[i].point, rot, transform);
        //         obj.transform.localScale = locScale;
        //     }
        // }


        var settings = GameObjectConversionSettings.FromWorld(World.DefaultGameObjectInjectionWorld, null);
        _entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        var entity = GameObjectConversionUtility.ConvertGameObjectHierarchy(treePrefab, settings);

        


        Vector3[] randPoses = new Vector3[amount];
        NativeArray<Vector3> res = new NativeArray<Vector3>(amount, Allocator.TempJob);

        var randJob = new DefineRandomPosJob() {
            randomMin = -250,
            randomMax = 250,
            rand = new Unity.Mathematics.Random(1),
            results = res
        };
        JobHandle randJobHandle = randJob.Schedule(amount, 64);
        randJobHandle.Complete();

        for (int i=0; i<amount; i++) {
            origins[i] = transform.TransformPoint(res[i]);
        }

        RaycastHit[] hits = PerformMultiRaycast(origins);
        for (int i=0; i<hits.Length; i++) {
            if (!string.IsNullOrEmpty(hits[i].collider.name)) {
                if (hits[i].collider.name != "NoFoliageCollider") {
                    Quaternion rot = Quaternion.Euler(0, UnityEngine.Random.Range(0, 360), 0);
                    float scale = UnityEngine.Random.Range(0.6f, 1.4f);
                    Vector3 locScale = Vector3.one * scale;
                    // GameObject obj = Instantiate(treePrefab, hits[i].point, rot, transform);
                    // obj.transform.localScale = locScale;

                    var entityInstance = _entityManager.Instantiate(entity);
                    float3 pos = hits[i].point;
                    _entityManager.SetComponentData(entityInstance, new Translation { Value = pos });
                    // _entityManager.SetSharedComponentData(entityInstance, new RenderMesh { mesh = prismMesh, material = prismMaterial });
                }
            }
        }

        res.Dispose();


       
    }

    private Vector3 GetSurfaceWorldPosition(Vector3 aWorldPosition) {
        RaycastHit hit;
        if (Physics.Raycast(aWorldPosition, Vector3.down, out hit, 500, 1<<0, QueryTriggerInteraction.Ignore)) {
            if (hit.collider.name != "NoFoliageCollider") {
                return hit.point;
            } else {
                return new Vector3(aWorldPosition.x, -99, aWorldPosition.z);
            }
        }
        return new Vector3(aWorldPosition.x, -99, aWorldPosition.z);
    }

    private RaycastHit[] PerformMultiRaycast(Vector3[] origins) {
        int count = origins.Length;
        NativeArray<RaycastHit> results = new NativeArray<RaycastHit>(count, Allocator.TempJob);
        NativeArray<RaycastCommand> commands = new NativeArray<RaycastCommand>(count, Allocator.TempJob);

        for (int i = 0; i<commands.Length; i++) {
            commands[i] = new RaycastCommand(origins[i], Vector3.down, 500, 1<<0, 1);
        }

        JobHandle handle = RaycastCommand.ScheduleBatch(commands, results, count, default(JobHandle));
        handle.Complete();

        RaycastHit[] batchedHits = new RaycastHit[count];
        for (int i=0; i<count; i++) {
            batchedHits[i] = results[i];
        }

        results.Dispose();
        commands.Dispose();
        
        return batchedHits;
    }

    [BurstCompile]
    private struct DefineRandomPosJob : IJobParallelFor {
        public float randomMin, randomMax;
        public Unity.Mathematics.Random rand;
        public NativeArray<Vector3> results;
        
        public void Execute(int index) {
            results[index] = new Vector3(rand.NextFloat(randomMin, randomMax), 0, rand.NextFloat(randomMin, randomMax));

        }
    }

}
