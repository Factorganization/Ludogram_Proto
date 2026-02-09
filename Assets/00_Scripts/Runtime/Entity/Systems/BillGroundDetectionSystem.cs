using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace BillSimulation
{
    [BurstCompile]
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [UpdateBefore(typeof(BillPhysicsSystem))]
    public partial struct BillGroundDetectionSystem : ISystem
    {
        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            float deltaTime = SystemAPI.Time.DeltaTime;
            
            // Récupérer le niveau du sol
            float groundLevel = 0.0f;
            if (SystemAPI.TryGetSingleton<GroundCollider>(out var ground) && ground.IsActive)
            {
                groundLevel = ground.Height;
            }
            
            float groundThreshold = 0.1f;
            
            foreach (var (transform, bill) in 
                SystemAPI.Query<RefRW<LocalTransform>, RefRW<BillData>>())
            {
                if (bill.ValueRO.State == BillState.Airborne)
                {
                    if (transform.ValueRO.Position.y <= groundLevel + groundThreshold)
                    {
                        bill.ValueRW.State = BillState.OnGround;
                        bill.ValueRW.TimeOnGround = 0;
                        
                        transform.ValueRW.Position.y = groundLevel;
                        
                        if (math.abs(bill.ValueRO.Velocity.y) > 2.0f)
                        {
                            bill.ValueRW.Velocity.y = math.abs(bill.ValueRO.Velocity.y) * 0.3f;
                            bill.ValueRW.State = BillState.Airborne;
                        }
                        else
                        {
                            bill.ValueRW.Velocity.y = 0;
                        }
                    }
                }
                else if (bill.ValueRO.State == BillState.OnGround)
                {
                    bill.ValueRW.TimeOnGround += deltaTime;
                    
                    transform.ValueRW.Position.y = groundLevel;
                    
                    float groundDrag = 3.0f;
                    bill.ValueRW.Velocity.x *= math.max(0, 1.0f - groundDrag * deltaTime);
                    bill.ValueRW.Velocity.z *= math.max(0, 1.0f - groundDrag * deltaTime);
                    bill.ValueRW.Velocity.y = 0;
                    
                    bill.ValueRW.AngularVelocity *= math.max(0, 1.0f - 5.0f * deltaTime);
                    
                    if (math.length(bill.ValueRO.Velocity) < 0.1f)
                    {
                        bill.ValueRW.Velocity = float3.zero;
                        bill.ValueRW.AngularVelocity = float3.zero;
                    }
                }
            }
        }
    }
}