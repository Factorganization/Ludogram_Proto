using Obi;

public class RopeComponent : PlayerComponent
{
    public ObiRope AttachedRope { get; private set; }
    
    
    public RopeComponent(PlayerCharacter player) : base(player)
    {
    }

    public void HandleRope()
    {
        if (!AttachedRope) return;
        
    }
    
    public void AttachRope(ObiRope rope)
    {
        character.IsAttached = true;
        AttachedRope = rope;
        //AttachedRope.stretchingScale = 2;
        
    }
    
    public void RemoveRope()
    {
        character.IsAttached = false;
        AttachedRope = null;
    }
    
    
}