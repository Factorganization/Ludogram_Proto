using Unity.Entities;
using UnityEngine;

namespace BillSimulation
{
    public class BillAuthoring : MonoBehaviour
    {
        public float CollisionRadius = 0.3f;
        
        class Baker : Baker<BillAuthoring>
        {
            public override void Bake(BillAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent<BillTag>(entity);
                AddComponent<BillData>(entity);
                AddComponent(entity, new GroundBillCollider
                {
                    Radius = authoring.CollisionRadius,
                    IsColliding = false
                });
            }
        }
    }
}