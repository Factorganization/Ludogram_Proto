using UnityEngine;

public abstract class PlayerBaseState : IState
{
    protected readonly PlayerCharacter character;
    protected readonly Animator animator;
        
    //protected static readonly int MovingHash = Animator.StringToHash("Locomotion");
    //protected static readonly int InteractingHash = Animator.StringToHash("Interacting");
        
    protected const float k_crossFadeDuration = 0.1f; 
        
    protected PlayerBaseState(PlayerCharacter character, Animator animator)
    {
        this.character = character;
        this.animator = animator;
    }
        
    public virtual void OnEnter() {}

    public virtual void Update() {}

    public virtual void FixedUpdate() {}
    public virtual void LateUpdate() { }

    public virtual void OnExit() {}

    public virtual void Dispose() {}
}    