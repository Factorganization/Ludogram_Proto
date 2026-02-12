using UnityEngine;


public class RopeRemover : MonoBehaviour
{
    [SerializeField] private RopeAttacher ropeAttacher;
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<PlayerCharacter>(out var player))
        {
            if (!player.IsAttached) return;
            
            ropeAttacher.FreeRope(player.Rope.AttachedRope);
            player.Rope.RemoveRope();
        }
    }
    
    
}