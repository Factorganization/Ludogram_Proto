using Obi;
using UnityEngine;

public class RopeAttacher : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private ObiRope rope;
    [SerializeField] private ObiParticleAttachment ropeAnchor;
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<PlayerCharacter>(out var player))
        {
            //rope.
            //ropeAnchor.target = player.transform;
        }
    }
}
