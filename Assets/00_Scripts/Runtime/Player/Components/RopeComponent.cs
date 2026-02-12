using Obi;

public class RopeComponent : PlayerComponent
{
    public ObiRope AttachedRope { get; private set; }
    public RopeTensionMonitor RopeTensionMonitor { get; private set; }
    
    private ObiRigidbody _attachedObiRigidbody;
    
    public RopeComponent(PlayerCharacter player) : base(player)
    {
    }

    public void HandleRope()
    {
        if (!AttachedRope) return;
        
        if (RopeTensionMonitor.IsUnderTension)
        {
            _attachedObiRigidbody.kinematicForParticles = true;
            AttachedRope.distanceConstraintsEnabled = true;
        }
        else
        {
            _attachedObiRigidbody.kinematicForParticles = false;
            AttachedRope.distanceConstraintsEnabled = false;
        }
        
        
    }

    public override void Initialize()
    {
        _attachedObiRigidbody = character.GetCachedComponent<ObiRigidbody>();
    }

    public void AttachRope(ObiRope rope)
    {
        character.IsAttached = true;
        AttachedRope = rope;
        RopeTensionMonitor = rope.GetComponent<RopeTensionMonitor>();

        RopeTensionMonitor.tensionThreshold = character.Playerstats.MaxRopeTension;
        
    }
    
    public void RemoveRope()
    {
        character.IsAttached = false;
        AttachedRope = null;
    }
    
    
}