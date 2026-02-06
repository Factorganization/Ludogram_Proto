using UnityEngine;

public abstract class PlayerBaseState : IState
{
    protected readonly PlayerCharacter character;
    protected readonly Animator animator;
        
    protected static readonly int LocomotionHash = Animator.StringToHash("Locomotion");
    protected static readonly int ShootHash = Animator.StringToHash("Shoot");
    protected static readonly int DashHash = Animator.StringToHash("Strike");
        
    protected const float k_crossFadeDuration = 0.1f; 
        
    protected PlayerBaseState(PlayerCharacter character, Animator animator)
    {
        this.character = character;
        this.animator = animator;
    }
        
    public virtual void OnEnter() {}

    public virtual void Update() {}

    public virtual void FixedUpdate() {}

    public virtual void OnExit() {}

    public virtual void Dispose() {}
}    