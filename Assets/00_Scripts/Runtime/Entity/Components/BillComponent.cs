using Unity.Entities;
using Unity.Mathematics;

namespace BillSimulation
{
    public enum BillState : byte
    {
        Airborne,
        OnGround,
        BeingKicked
    }

    public struct BillData : IComponentData
    {
        public float3 Velocity;
        public float3 AngularVelocity;
        public float Mass;
        public float Drag;
        public float AngularDrag;
        public float LifeTime;
        public float CurrentTime;
        public BillState State;
        public float GroundFriction;
        public float TimeOnGround;
    }

    public struct BillTag : IComponentData { }

    public struct GroundBillCollider : IComponentData
    {
        public float Radius;
        public bool IsColliding;
    }
}