using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;

namespace BillSimulation
{
    [BurstCompile]
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [UpdateAfter(typeof(BillPlayerCollisionSystem))]
    public partial struct BillStateTransitionSystem : ISystem
    {
        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            foreach (var (bill, transform) in 
                     SystemAPI.Query<RefRW<BillData>, RefRO<LocalTransform>>())
            {
                if (bill.ValueRO.State == BillState.BeingKicked)
                {
                    if (transform.ValueRO.Position.y > 0.1f)
                    {
                        bill.ValueRW.State = BillState.Airborne;
                    }
                }
            }
        }
    }
}