using MortierFu.Shared;
using UnityEngine;

public class MovingState : PlayerBaseState
{
    public MovingState(PlayerCharacter character, Animator animator) : base(character, animator)
    { }

    public override void OnEnter()
    {
        Logs.Log($"[Player {character.PlayerIndex}] Moving state entered");
        //animator?.CrossFade(MovingHash, k_crossFadeDuration);
    }
        
    public override void Update()
    {
        character.Controller.HandleMovementUpdate();
    }
    
        
    public override void OnExit()
    {
        Logs.Log($"[Player {character.PlayerIndex}] Moving state exited");
    }
}