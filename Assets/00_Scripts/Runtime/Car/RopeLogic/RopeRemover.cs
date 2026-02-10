using UnityEngine;


public class RopeRemover : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<PlayerCharacter>(out var player))
        {
            if (!player.IsAttached) return;
            player.Rope.RemoveRope();
        }
    }
}