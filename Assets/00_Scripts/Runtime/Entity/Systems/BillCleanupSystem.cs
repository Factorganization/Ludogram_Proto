using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;
using Unity.Collections;
using Unity.Mathematics;

namespace BillSimulation
{
    [BurstCompile]
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [UpdateAfter(typeof(BillPhysicsSystem))]
    public partial struct BillCleanupSystem : ISystem
    {
        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var ecb = new EntityCommandBuffer(Allocator.Temp);
            
            foreach (var (bill, transform, entity) in 
                     SystemAPI.Query<RefRO<BillData>, RefRO<LocalTransform>>().WithEntityAccess())
            {
                bool tooLow = transform.ValueRO.Position.y < -10f;
                bool expired = bill.ValueRO.CurrentTime >= bill.ValueRO.LifeTime;
                bool isMoving = math.length(bill.ValueRO.Velocity) > 0.01f;
                
                
                if (tooLow || (expired && isMoving))
                {
                    ecb.DestroyEntity(entity);
                }
            }
            
            ecb.Playback(state.EntityManager);
            ecb.Dispose();
        }
    }
}