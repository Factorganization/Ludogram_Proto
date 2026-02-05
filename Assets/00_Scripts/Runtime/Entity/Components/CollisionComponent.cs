using Unity.Entities;
using Unity.Mathematics;

namespace BillSimulation
{
    // Tag pour les murs avec lesquels les billets peuvent collider
    public struct WallCollider : IComponentData
    {
        public float3 Position;
        public float3 HalfExtents; // Pour un box collider
        public quaternion Rotation;
    }
    
    // Component pour activer les collisions entre billets
    public struct BillBillCollisionEnabled : IComponentData
    {
        public bool Enabled;
    }
    
    // NOUVEAU : Component pour le sol
    public struct GroundCollider : IComponentData
    {
        public float Height; // Niveau Y du sol
        public bool IsActive;
    }
}