using Unity.Entities;
using Unity.Collections;
using UnityEngine;

namespace BillSimulation
{
    public class TriggerBillSpawn : MonoBehaviour
    {
        [Header("Controls")]
        [Tooltip("Touche pour faire spawner les billets")]
        public KeyCode SpawnKey = KeyCode.Space;
        
        void Update()
        {
            if (Input.GetKeyDown(SpawnKey))
            {
                var entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
                var query = entityManager.CreateEntityQuery(typeof(BillSpawner));
                var spawners = query.ToComponentDataArray<BillSpawner>(Allocator.Temp);
                var entities = query.ToEntityArray(Allocator.Temp);
                
                for (int i = 0; i < spawners.Length; i++)
                {
                    var spawner = spawners[i];
                    spawner.ShouldSpawn = true;
                    entityManager.SetComponentData(entities[i], spawner);
                }
                
                spawners.Dispose();
                entities.Dispose();
                
                Debug.Log("Billets spawned!");
            }
        }
    }
}