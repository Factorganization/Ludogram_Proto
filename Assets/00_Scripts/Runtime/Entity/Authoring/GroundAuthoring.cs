using Unity.Entities;
using UnityEngine;

namespace BillSimulation
{
    public class GroundAuthoring : MonoBehaviour
    {
        [Header("Ground Settings")]
        [Tooltip("Hauteur du sol (généralement la position Y de ce GameObject)")]
        public float GroundHeight = 0f;
        
        [Tooltip("Utiliser la position Y du GameObject")]
        public bool UseTransformY = true;
        
        class Baker : Baker<GroundAuthoring>
        {
            public override void Bake(GroundAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.None);
                
                float height = authoring.UseTransformY 
                    ? authoring.transform.position.y 
                    : authoring.GroundHeight;
                
                AddComponent(entity, new GroundCollider
                {
                    Height = height,
                    IsActive = true
                });
            }
        }
        
        void OnDrawGizmos()
        {
            float height = UseTransformY ? transform.position.y : GroundHeight;
            
            Gizmos.color = new Color(0, 1, 0, 0.1f);
            Gizmos.DrawCube(new Vector3(0, height, 0), new Vector3(100, 0.1f, 100));
            
            Gizmos.color = new Color(0, 1, 0, 0.5f);
            Gizmos.DrawWireCube(new Vector3(0, height, 0), new Vector3(100, 0.1f, 100));
        }
    }
}