using Unity.Entities;
using Unity.Mathematics;

namespace BillSimulation
{
    public struct PlayerTag : IComponentData { }

    public struct PlayerMovement : IComponentData
    {
        public float3 Position;
        public float3 Velocity;
        public float CollisionRadius;
    }
}