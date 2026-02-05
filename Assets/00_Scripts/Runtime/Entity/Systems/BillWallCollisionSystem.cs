using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Collections;

namespace BillSimulation
{
    [BurstCompile]
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [UpdateAfter(typeof(BillPhysicsSystem))]
    public partial struct BillWallCollisionSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<WallCollider>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            // Récupérer tous les murs
            var wallQuery = SystemAPI.QueryBuilder()
                .WithAll<WallCollider>()
                .Build();
            
            var walls = wallQuery.ToComponentDataArray<WallCollider>(Allocator.Temp);
            
            // Pour chaque billet, vérifier collision avec chaque mur
            foreach (var (transform, bill, collider) in 
                SystemAPI.Query<RefRW<LocalTransform>, RefRW<BillData>, RefRO<GroundBillCollider>>())
            {
                float3 billPos = transform.ValueRO.Position;
                float billRadius = collider.ValueRO.Radius;
                
                for (int i = 0; i < walls.Length; i++)
                {
                    WallCollider wall = walls[i];
                    
                    // Calculer le point le plus proche sur le box collider du mur
                    float3 localPoint = math.mul(math.inverse(wall.Rotation), billPos - wall.Position);
                    
                    // Clamper au box
                    float3 closestLocal = math.clamp(localPoint, -wall.HalfExtents, wall.HalfExtents);
                    float3 closestWorld = math.mul(wall.Rotation, closestLocal) + wall.Position;
                    
                    float3 delta = billPos - closestWorld;
                    delta.y = 0; // Collision 2D
                    
                    float distance = math.length(delta);
                    
                    if (distance < billRadius && distance > 0.001f)
                    {
                        // Collision avec le mur
                        float3 normal = math.normalize(delta);
                        float overlap = billRadius - distance;
                        
                        // Pousser le billet hors du mur
                        transform.ValueRW.Position += normal * overlap;
                        
                        // Bounce effect
                        float3 velocity = bill.ValueRO.Velocity;
                        float dotProduct = math.dot(velocity, normal);
                        
                        if (dotProduct < 0) // S'approche du mur
                        {
                            // Réflexion de la vélocité avec amortissement
                            velocity -= normal * dotProduct * 1.5f; // 1.5 pour un bounce léger
                            bill.ValueRW.Velocity = velocity * 0.7f; // Perte d'énergie
                        }
                    }
                }
            }
            
            walls.Dispose();
        }
    }
}