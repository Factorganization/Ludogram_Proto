using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Collections;

namespace BillSimulation
{
    [BurstCompile]
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [UpdateAfter(typeof(BillGroundDetectionSystem))]
    public partial struct BillPlayerCollisionSystem : ISystem
    {
        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            if (!SystemAPI.TryGetSingleton<PlayerMovement>(out var player))
                return;
            
            float3 playerPos = player.Position;
            float3 playerVelocity = player.Velocity;
            float playerRadius = player.CollisionRadius;
            
            foreach (var (transform, bill, collider) in 
                SystemAPI.Query<RefRW<LocalTransform>, RefRW<BillData>, RefRW<GroundBillCollider>>())
            {
                if (bill.ValueRO.State != BillState.OnGround)
                {
                    collider.ValueRW.IsColliding = false;
                    continue;
                }
                
                float3 billPos = transform.ValueRO.Position;
                float3 toBill = billPos - playerPos;
                toBill.y = 0; // Collision en 2D horizontal
                
                float distance = math.length(toBill);
                float combinedRadius = playerRadius + collider.ValueRO.Radius;
                
                if (distance < combinedRadius && distance > 0.001f)
                {
                    collider.ValueRW.IsColliding = true;
                    bill.ValueRW.State = BillState.BeingKicked;
                    
                    // Direction de poussée
                    float3 pushDirection = math.normalize(toBill);
                    
                    // Force basée sur la vitesse du joueur + force minimale
                    float playerSpeed = math.length(playerVelocity);
                    float pushForce = 3f;
                    
                    // Vélocité de kick avec composante verticale
                    float3 kickVelocity = pushDirection * pushForce;
                    kickVelocity.y = math.max(2.0f, pushForce * 0.4f); // Plus de hauteur
                    
                    bill.ValueRW.Velocity = kickVelocity;
                    
                    // Rotation aléatoire 
                    var random = Random.CreateFromIndex((uint)(SystemAPI.Time.ElapsedTime * 1000 + distance * 100));
                    bill.ValueRW.AngularVelocity = random.NextFloat3(-15f, 15f);
                }
                else
                {
                    collider.ValueRW.IsColliding = false;
                }
            }
        }
    }
}