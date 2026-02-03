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
            float groundLevel = 0.0f;
            
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
                // ptit vent de mistral
                float3 turbulence = new float3(
                    noise.snoise(new float2(transform.Position.x * 0.5f, Time * 0.3f)),
                    noise.snoise(new float2(transform.Position.y * 0.5f, Time * 0.3f + 100)),
                    noise.snoise(new float2(transform.Position.z * 0.5f, Time * 0.3f + 200))
                ) * 1.5f;
                
                float3 force = Gravity * bill.Mass + turbulence;
                float3 acceleration = force / bill.Mass;
                
                bill.Velocity += acceleration * DeltaTime;
                bill.Velocity *= math.max(0, 1.0f - bill.Drag * DeltaTime);
                
                // Mise à jour position
                float3 newPosition = transform.Position + bill.Velocity * DeltaTime;
                
                // Empêcher de traverser le sol
                if (newPosition.y < GroundLevel)
                {
                    newPosition.y = GroundLevel;
                }
                
                transform.Position = newPosition;
                
                // Rotation
                bill.AngularVelocity *= math.max(0, 1.0f - bill.AngularDrag * DeltaTime);
                quaternion deltaRotation = quaternion.EulerXYZ(bill.AngularVelocity * DeltaTime);
                transform.Rotation = math.mul(transform.Rotation, deltaRotation);
            }
            else if (bill.State == BillState.OnGround)
            {
                // Mouvement au sol (glissement)
                float3 horizontalVelocity = new float3(bill.Velocity.x, 0, bill.Velocity.z);
                transform.Position += horizontalVelocity * DeltaTime;
                
                // forcer au niveau du sol
                transform.Position.y = GroundLevel;
                
                // Rotation minimale au sol
                if (math.length(bill.AngularVelocity) > 0.01f)
                {
                    quaternion deltaRotation = quaternion.EulerXYZ(bill.AngularVelocity * DeltaTime);
                    transform.Rotation = math.mul(transform.Rotation, deltaRotation);
                }
            }
        }
    }
}