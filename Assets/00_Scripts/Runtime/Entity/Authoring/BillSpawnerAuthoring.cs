using Unity.Entities;
using UnityEngine;

namespace BillSimulation
{
    public class BillSpawnerAuthoring : MonoBehaviour
    {
        [Header("Prefab Settings")]
        public GameObject BillPrefab;
        
        [Header("Spawn Settings")]
        public int NumberOfBills = 1000;
        public float SpawnRadius = 2f;
        public float InitialForceMin = 5f;
        public float InitialForceMax = 15f;
        
        class Baker : Baker<BillSpawnerAuthoring>
        {
            public override void Bake(BillSpawnerAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.None);
                
                if (authoring.BillPrefab == null)
                {
                    Debug.LogError("BillPrefab is null in BillSpawnerAuthoring! Please assign a prefab in the Inspector.");
                    return;
                }
                
                AddComponent(entity, new BillSpawner
                {
                    BillPrefab = GetEntity(authoring.BillPrefab, TransformUsageFlags.Dynamic),
                    SpawnPosition = authoring.transform.position,
                    SpawnRadius = authoring.SpawnRadius,
                    BillsToSpawn = authoring.NumberOfBills,
                    InitialForceMin = authoring.InitialForceMin,
                    InitialForceMax = authoring.InitialForceMax,
                    ShouldSpawn = false
                });
            }
        }
    }
}