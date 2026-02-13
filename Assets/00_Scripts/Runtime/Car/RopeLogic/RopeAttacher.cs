using System;
using MortierFu.Shared;
using Obi;
using UnityEngine;

public class RopeAttacher : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private SpringJoint[] ropeSpringJoint;
    
    private bool rope1Used = false;
    private bool rope2Used = false;
    private bool rope3Used = false;
    private bool rope4Used = false;
    
    private SimpleRopeRenderer[] _ropeRenderer = new SimpleRopeRenderer[3];
    [SerializeField] private Transform _ropeAnchorPoint;
    

    private void Start()
    {
        // Populate ropeSpringJoint array by getting the 4 springJoint component from THIS gameObject
        ropeSpringJoint = GetComponents<SpringJoint>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<PlayerCharacter>(out var player))
        {
            if (player.IsAttached) return;
            
            int ropeIndex = ChoseRopeToAttach();
            if (ropeIndex == -9999) return; // No available ropes
            
            // Connect the rope's spring joint to the player's Rigidbody
            ropeSpringJoint[ropeIndex].connectedBody = player.GetCachedComponent<Rigidbody>();
            ropeSpringJoint[ropeIndex].maxDistance = player.Playerstats.MaxRopeDistance;
            player.RopeRenderer.endPoint = _ropeAnchorPoint;
            _ropeRenderer[ropeIndex] = player.RopeRenderer;
            player.Rope.AttachRope(ropeSpringJoint[ropeIndex], this); 
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
    
    public void FreeRope(SpringJoint ropeToFree )
    {
        for (int i = 0; i < ropeSpringJoint.Length; i++)
        {
            if (ropeSpringJoint[i] == ropeToFree)
            {
                ropeSpringJoint[i].connectedBody = null;
                _ropeRenderer[i].endPoint = null;
                
                switch (i)
                {
                    case 0: rope1Used = false; break;
                    case 1: rope2Used = false; break;
                    case 2: rope3Used = false; break;
                    case 3: rope4Used = false; break;
                }
                return;
            }
        }
        
        Logs.LogError("Attempted to free a rope that is not managed by this RopeAttacher!");
    }
    
}
