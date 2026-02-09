using MortierFu.Shared;
using UnityEngine;

public class DrivingState : PlayerBaseState
{
    public DrivingState(PlayerCharacter character, Animator animator) : base(character, animator)
    { }

    public override void OnEnter()
    {
        Logs.Log($"[Player {character.PlayerIndex}] Driving state entered");
        animator?.CrossFade("Driving", k_crossFadeDuration);
    }
        
    public override void Update()
    {
        character.Driving.HandleDrivingUpdate();
        character.Interact.HandleInteractUpdate();
    }
    
        
    public override void OnExit()
    {
        Logs.Log($"[Player {character.PlayerIndex}] Driving state exited");
    }
}