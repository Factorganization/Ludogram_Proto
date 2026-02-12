using MortierFu.Shared;
using Obi;
using UnityEngine;

public class RopeAttacher : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private ObiRope[] rope;
    
    private bool rope1Used = false;
    private bool rope2Used = false;
    private bool rope3Used = false;
    private bool rope4Used = false;
    
    
    [SerializeField] private ObiParticleAttachment[] ropeAnchor;
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<PlayerCharacter>(out var player))
        {
            if (player.IsAttached) return;
            
            int ropeIndex = ChoseRopeToAttach();
            if (ropeIndex == -9999) return; // No available ropes
            
            player.Rope.AttachRope(rope[ropeIndex]);
            
            ropeAnchor[ropeIndex].target =player.transform;
            
            rope[ropeIndex].gameObject.SetActive(true);
        }
    }

    private int ChoseRopeToAttach()
    {
        if (!rope1Used)
        {
            rope1Used = true;
            return 0;
        }
        if (!rope2Used)
        {
            rope2Used = true;
            return 1;
        }
        if (!rope3Used)
        {
            rope3Used = true;
            return 2;
        }
        if (!rope4Used)
        {
            rope4Used = true;
            return 3;
        }
        
        Logs.LogError("All ropes are currently in use! Cannot attach another player.");
        return -9999; // No available ropes
    }
    
    public void FreeRope(ObiRope ropeToFree)
    {
        if (ropeToFree == rope[0])
        {
            rope1Used = false;
            rope[0].gameObject.SetActive(false);
        }
        else if (ropeToFree == rope[1]) 
        { 
            rope2Used = false; 
            rope[1].gameObject.SetActive(false);
        } 
        else if (ropeToFree == rope[2])
        {
            rope3Used = false;
            rope[2].gameObject.SetActive(false);
        } 
        else if (ropeToFree == rope[3])
        {
            rope4Used = false;
            rope[3].gameObject.SetActive(false);
        }
    }
    
}
