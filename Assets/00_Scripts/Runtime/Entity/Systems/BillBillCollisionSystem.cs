using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Collections;

namespace BillSimulation
{
    // PAS de BurstCompile pour pouvoir modifier les components
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [UpdateAfter(typeof(BillGroundDetectionSystem))]
    [UpdateBefore(typeof(BillPlayerCollisionSystem))]
    public partial struct BillBillCollisionSystem : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<BillBillCollisionEnabled>();
        }

        public void OnUpdate(ref SystemState state)
        {
            if (!SystemAPI.TryGetSingleton<BillBillCollisionEnabled>(out var config))
                return;
            
            if (!config.Enabled)
                return;
            
            // Collecter tous les billets au sol dans des listes
            var groundBills = new NativeList<Entity>(Allocator.Temp);
            var positions = new NativeList<float3>(Allocator.Temp);
            var radii = new NativeList<float>(Allocator.Temp);
            
            foreach (var (transform, bill, collider, entity) in 
                SystemAPI.Query<RefRO<LocalTransform>, RefRO<BillData>, RefRO<GroundBillCollider>>()
                .WithEntityAccess())
            {
                if (bill.ValueRO.State == BillState.OnGround)
                {
                    groundBills.Add(entity);
                    positions.Add(transform.ValueRO.Position);
                    radii.Add(collider.ValueRO.Radius);
                }
            }
            
            // Vérifier collisions
            for (int i = 0; i < groundBills.Length; i++)
            {
                for (int j = i + 1; j < groundBills.Length; j++)
                {
                    float3 posA = positions[i];
                    float3 posB = positions[j];
                    
                    float3 delta = posB - posA;
                    delta.y = 0; // Collision 2D horizontale
                    
                    float distance = math.length(delta);
                    float combinedRadius = radii[i] + radii[j];
                    
                    if (distance < combinedRadius && distance > 0.001f)
                    {
                        // Collision détectée
                        float overlap = combinedRadius - distance;
                        float3 separationDir = math.normalize(delta);
                        float3 separation = separationDir * (overlap * 0.5f);
                        
                        // Récupérer et modifier les components
                        var transformA = state.EntityManager.GetComponentData<LocalTransform>(groundBills[i]);
                        var transformB = state.EntityManager.GetComponentData<LocalTransform>(groundBills[j]);
                        var billA = state.EntityManager.GetComponentData<BillData>(groundBills[i]);
                        var billB = state.EntityManager.GetComponentData<BillData>(groundBills[j]);
                        
                        // Séparer les positions
                        transformA.Position -= separation;
                        transformB.Position += separation;
                        
                        // Mise à jour positions dans la liste pour les prochaines itérations
                        positions[i] = transformA.Position;
                        positions[j] = transformB.Position;
                        
                        // Transférer de la vélocité
                        float3 relativeVelocity = billB.Velocity - billA.Velocity;
                        float velocityAlongNormal = math.dot(relativeVelocity, separationDir);
                        
                        if (velocityAlongNormal < 0) // S'approchent l'un de l'autre
                        {
                            float3 impulse = separationDir * velocityAlongNormal * 0.5f;
                            billA.Velocity += impulse;
                            billB.Velocity -= impulse;
                        }
                        
                        // Appliquer les changements
                        state.EntityManager.SetComponentData(groundBills[i], transformA);
                        state.EntityManager.SetComponentData(groundBills[j], transformB);
                        state.EntityManager.SetComponentData(groundBills[i], billA);
                        state.EntityManager.SetComponentData(groundBills[j], billB);
                    }
                }
            }
            
            groundBills.Dispose();
            positions.Dispose();
            radii.Dispose();
        }
    }
}