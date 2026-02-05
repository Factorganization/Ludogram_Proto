using Unity.Entities;
using UnityEngine;

namespace BillSimulation
{
    public class CollisionConfig : MonoBehaviour
    {
        [Header("Collision Settings")]
        [Tooltip("Activer les collisions entre billets (peut réduire les perfs si beaucoup de billets)")]
        public bool EnableBillBillCollisions = true;
        
        private Entity configEntity;
        private EntityManager entityManager;
        
        void Start()
        {
            entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
            
            configEntity = entityManager.CreateEntity(typeof(BillBillCollisionEnabled));
            entityManager.SetComponentData(configEntity, new BillBillCollisionEnabled
            {
                Enabled = EnableBillBillCollisions
            });
        }
        
        void OnDestroy()
        {
            if (entityManager != null && entityManager.Exists(configEntity))
            {
                entityManager.DestroyEntity(configEntity);
            }
        }
        
        // Pour toggle en runtime si besoin
        public void ToggleBillCollisions(bool enabled)
        {
            EnableBillBillCollisions = enabled;
            
            if (entityManager.Exists(configEntity))
            {
                entityManager.SetComponentData(configEntity, new BillBillCollisionEnabled
                {
                    Enabled = enabled
                });
            }
        }
    }
}
