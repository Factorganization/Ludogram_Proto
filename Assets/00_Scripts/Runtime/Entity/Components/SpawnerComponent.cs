using Unity.Entities;
using Unity.Mathematics;

namespace BillSimulation
{
    public struct BillSpawner : IComponentData
    {
        public Entity BillPrefab;
        public float3 SpawnPosition;
        public float SpawnRadius;
        public int BillsToSpawn;
        public float InitialForceMin;
        public float InitialForceMax;
        public bool ShouldSpawn;
    }
}