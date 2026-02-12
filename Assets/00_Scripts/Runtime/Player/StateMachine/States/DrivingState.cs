using MortierFu.Shared;
using UnityEngine;

public class DrivingState : PlayerBaseState
{
    public DrivingState(PlayerCharacter character, Animator animator) : base(character, animator)
    { }

    public override void OnEnter()
    {
        Logs.Log($"[Player {character.PlayerIndex}] Driving state entered");
        // TODO animator?.CrossFade("Driving", k_crossFadeDuration);
        
        character.SwitchInputMap("Driving");
        character._uiSwapper.SwitchCrosshairVisibility(false);
        
    }
        
    public override void Update()
    {
        character.Driving.HandleDrivingUpdate();
        character.Interact.HandleInteractUpdate();
        character.Controller.HandleMovementUpdate();
    }

    public override void LateUpdate()
    {
        character.Controller.HandleLookUpdate();
    }


    public override void OnExit()
    {
        Logs.Log($"[Player {character.PlayerIndex}] Driving state exited");
        character.SwitchInputMap("Player");
        character._uiSwapper.SwitchCrosshairVisibility(true);
    }
}