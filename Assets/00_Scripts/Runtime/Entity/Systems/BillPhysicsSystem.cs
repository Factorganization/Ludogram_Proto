using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace BillSimulation
{
    [BurstCompile]
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    public partial struct BillPhysicsSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<BillData>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            float deltaTime = SystemAPI.Time.DeltaTime;
            float3 gravity = new float3(0, -9.81f, 0);
            float time = (float)SystemAPI.Time.ElapsedTime;
            
            // Récupérer le niveau du sol depuis le GroundCollider
            float groundLevel = 0.0f;
            if (SystemAPI.TryGetSingleton<GroundCollider>(out var ground) && ground.IsActive)
            {
                groundLevel = ground.Height;
            }
            
            var job = new BillPhysicsJob
            {
                DeltaTime = deltaTime,
                Gravity = gravity,
                Time = time,
                GroundLevel = groundLevel
            };
            
            job.ScheduleParallel();
        }
    }

    [BurstCompile]
    public partial struct BillPhysicsJob : IJobEntity
    {
        public float DeltaTime;
        public float3 Gravity;
        public float Time;
        public float GroundLevel;
        
        private void Execute(ref LocalTransform transform, ref BillData bill)
        {
            bill.CurrentTime += DeltaTime;
            
            if (bill.State == BillState.Airborne || bill.State == BillState.BeingKicked)
            {
                // Turbulence SEULEMENT si on est assez haut et en mouvement
                float3 turbulence = float3.zero;
                
                if (transform.Position.y > GroundLevel + 0.5f && math.length(bill.Velocity) > 1.0f)
                {
                    turbulence = new float3(
                        noise.snoise(new float2(transform.Position.x * 0.5f, Time * 0.3f)),
                        noise.snoise(new float2(transform.Position.y * 0.5f, Time * 0.3f + 100)),
                        noise.snoise(new float2(transform.Position.z * 0.5f, Time * 0.3f + 200))
                    ) * 0.5f;
                }
                
                float3 force = Gravity * bill.Mass + turbulence;
                float3 acceleration = force / bill.Mass;
                
                bill.Velocity += acceleration * DeltaTime;
                bill.Velocity *= math.max(0, 1.0f - bill.Drag * DeltaTime);
                
                float3 newPosition = transform.Position + bill.Velocity * DeltaTime;
                
                if (newPosition.y < GroundLevel)
                {
                    newPosition.y = GroundLevel;
                }
                
                transform.Position = newPosition;
                
                bill.AngularVelocity *= math.max(0, 1.0f - bill.AngularDrag * DeltaTime);
                quaternion deltaRotation = quaternion.EulerXYZ(bill.AngularVelocity * DeltaTime);
                transform.Rotation = math.mul(transform.Rotation, deltaRotation);
            }
            else if (bill.State == BillState.OnGround)
            {
                float3 horizontalVelocity = new float3(bill.Velocity.x, 0, bill.Velocity.z);
                transform.Position += horizontalVelocity * DeltaTime;
                
                transform.Position.y = GroundLevel;
                
                if (math.length(bill.AngularVelocity) > 0.01f)
                {
                    quaternion deltaRotation = quaternion.EulerXYZ(bill.AngularVelocity * DeltaTime);
                    transform.Rotation = math.mul(transform.Rotation, deltaRotation);
                }
            }
        }
    }
}