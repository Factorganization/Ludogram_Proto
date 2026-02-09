using MortierFu.Shared;
using UnityEngine;

public class FlyingState : PlayerBaseState
{
    public FlyingState(PlayerCharacter character, Animator animator) : base(character, animator)
    { }

    public override void OnEnter()
    {
        Logs.Log($"[Player {character.PlayerIndex}] Flying state entered");
        //animator?.CrossFade(MovingHash, k_crossFadeDuration);
    }
        
    public override void Update()
    {
        // TODO: Handle flying input and movement 
    }
    
        
    public override void OnExit()
    {
        Logs.Log($"[Player {character.PlayerIndex}] Flying state exited");
    }
}