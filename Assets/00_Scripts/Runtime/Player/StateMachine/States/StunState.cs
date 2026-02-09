using MortierFu.Shared;
using UnityEngine;

public class StunState : PlayerBaseState
{
    public StunState(PlayerCharacter character, Animator animator) : base(character, animator)
    {
    }
    
    public override void OnEnter()
    {
        Logs.Log($"[Player {character.PlayerIndex}] Stun state entered");
        animator?.CrossFade("Stun", k_crossFadeDuration);
        character.Controller.ResetVelocity();
        
        // TODO: Set Camera to ThirdPerson
    }

    public override void OnExit()
    {
        Logs.Log($"[Player {character.PlayerIndex}] Stun state exited");
        // TODO: Set Camera back to FirstPerson
    }
}