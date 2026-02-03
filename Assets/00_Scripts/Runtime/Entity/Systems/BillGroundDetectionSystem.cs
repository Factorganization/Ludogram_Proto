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
            float groundLevel = -1f; // Niveau du sol
            float groundThreshold = 0.1f; 
            
            foreach (var (transform, bill) in 
                SystemAPI.Query<RefRW<LocalTransform>, RefRW<BillData>>())
            {
                if (bill.ValueRO.State == BillState.Airborne)
                {
                    // Détection du sol
                    if (transform.ValueRO.Position.y <= groundLevel + groundThreshold)
                    {
                        bill.ValueRW.State = BillState.OnGround;
                        bill.ValueRW.TimeOnGround = 0;
                        
                        // FORCER la position exactement au sol
                        transform.ValueRW.Position.y = groundLevel;
                        
                        // Rebond léger si la vitesse verticale est assez grande
                        if (math.abs(bill.ValueRO.Velocity.y) > 2.0f)
                        {
                            bill.ValueRW.Velocity.y = math.abs(bill.ValueRO.Velocity.y) * 0.3f;
                            // Si on rebondit, retour en airborne
                            bill.ValueRW.State = BillState.Airborne;
                        }
                        else
                        {
                            // Sinon on reste au sol
                            bill.ValueRW.Velocity.y = 0;
                        }
                    }
                }
                else if (bill.ValueRO.State == BillState.OnGround)
                {
                    bill.ValueRW.TimeOnGround += deltaTime;
                    
                    
                    transform.ValueRW.Position.y = groundLevel;
                    
                    // Appliquer la friction au sol
                    float groundDrag = 3.0f;
                    bill.ValueRW.Velocity.x *= math.max(0, 1.0f - groundDrag * deltaTime);
                    bill.ValueRW.Velocity.z *= math.max(0, 1.0f - groundDrag * deltaTime);
                    bill.ValueRW.Velocity.y = 0; 
                    
                    // Ralentir la rotation
                    bill.ValueRW.AngularVelocity *= math.max(0, 1.0f - 5.0f * deltaTime);
                    
                    // Si presque immobile, arrêter complètement
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