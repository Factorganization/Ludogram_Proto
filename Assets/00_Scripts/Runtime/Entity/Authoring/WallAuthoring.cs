using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace BillSimulation
{
    public class WallAuthoring : MonoBehaviour
    {
        [Header("Wall Collision Settings")]
        [Tooltip("Le collider utilisera la taille du GameObject")]
        public bool UseObjectScale = true;
        
        [Tooltip("Taille custom du collider (si UseObjectScale = false)")]
        public Vector3 CustomSize = new Vector3(1, 2, 0.1f);
        
        class Baker : Baker<WallAuthoring>
        {
            public override void Bake(WallAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                
                Vector3 size = authoring.UseObjectScale 
                    ? authoring.transform.localScale 
                    : authoring.CustomSize;
                
                AddComponent(entity, new WallCollider
                {
                    Position = authoring.transform.position,
                    HalfExtents = size * 0.5f,
                    Rotation = authoring.transform.rotation
                });
            }
        }
        
        // Visualisation dans l'éditeur
        void OnDrawGizmos()
        {
            Gizmos.color = new Color(1, 0, 0, 0.3f);
            Gizmos.matrix = transform.localToWorldMatrix;
            
            Vector3 size = UseObjectScale ? Vector3.one : CustomSize;
            Gizmos.DrawCube(Vector3.zero, size);
            
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(Vector3.zero, size);
        }
        
        void OnDrawGizmosSelected()
        {
            Gizmos.color = new Color(1, 0, 0, 0.5f);
            Gizmos.matrix = transform.localToWorldMatrix;
            
            Vector3 size = UseObjectScale ? Vector3.one : CustomSize;
            Gizmos.DrawCube(Vector3.zero, size);
        }
    }
}