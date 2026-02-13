using Obi;
using UnityEngine;

public class RopeComponent : PlayerComponent
{
    public SpringJoint AttachedRope { get; private set; }
    public RopeAttacher RopeAttacher { get; private set; }
    
    private ObiRigidbody _attachedObiRigidbody;
    
    public RopeComponent(PlayerCharacter player) : base(player)
    {
    }

    public void HandleRope()
    {
        //_attachedObiRigidbody.kinematicForParticles = !RopeTensionMonitor.IsUnderTension;
        //AttachedRope.distanceConstraintsEnabled = true;
        //AttachedRope.distanceConstraintsEnabled = false;
    }

    public override void Initialize()
    {
        _attachedObiRigidbody = character.GetCachedComponent<ObiRigidbody>();
    }

    public void AttachRope(SpringJoint rope, RopeAttacher ropeAttacher)
    {
        character.IsAttached = true;
        AttachedRope = rope;
        RopeAttacher = ropeAttacher;
    }
    
    public void RemoveRope()
    {
        character.IsAttached = false;
        AttachedRope = null;
        RopeAttacher = null;
    }
    
    
}