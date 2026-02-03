using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Collections;

namespace BillSimulation
{
    [BurstCompile]
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    public partial struct BillSpawnerSystem : ISystem
    {
        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var ecb = new EntityCommandBuffer(Allocator.Temp);
            
            foreach (var (spawner, entity) in 
                SystemAPI.Query<RefRW<BillSpawner>>().WithEntityAccess())
            {
                if (!spawner.ValueRO.ShouldSpawn) continue;
                
                var random = Random.CreateFromIndex((uint)(SystemAPI.Time.ElapsedTime * 1000));
                
                for (int i = 0; i < spawner.ValueRO.BillsToSpawn; i++)
                {
                    Entity bill = ecb.Instantiate(spawner.ValueRO.BillPrefab);
                    
                    float angle = random.NextFloat(0, math.PI * 2);
                    float radius = random.NextFloat(0, spawner.ValueRO.SpawnRadius);
                    float3 offset = new float3(
                        math.cos(angle) * radius,
                        random.NextFloat(-0.5f, 0.5f),
                        math.sin(angle) * radius
                    );
                    
                    float3 spawnPos = spawner.ValueRO.SpawnPosition + offset;
                    
                    float force = random.NextFloat(
                        spawner.ValueRO.InitialForceMin, 
                        spawner.ValueRO.InitialForceMax
                    );
                    
                    float3 direction = new float3(
                        random.NextFloat(-0.3f, 0.3f),
                        1.0f,
                        random.NextFloat(-0.3f, 0.3f)
                    );
                    direction = math.normalize(direction);
                    
                    ecb.SetComponent(bill, new LocalTransform
                    {
                        Position = spawnPos,
                        Rotation = quaternion.identity,
                        Scale = 1.0f
                    });
                    
                    ecb.SetComponent(bill, new BillData
                    {
                        Velocity = direction * force,
                        AngularVelocity = random.NextFloat3(-5f, 5f),
                        Mass = random.NextFloat(0.8f, 1.2f),
                        Drag = 0.5f,
                        AngularDrag = 2.0f,
                        LifeTime = 300.0f, 
                        CurrentTime = 0,
                        State = BillState.Airborne,
                        GroundFriction = 3.0f,
                        TimeOnGround = 0
                    });
                }
                
                spawner.ValueRW.ShouldSpawn = false;
            }
            
            ecb.Playback(state.EntityManager);
            ecb.Dispose();
        }
    }
}