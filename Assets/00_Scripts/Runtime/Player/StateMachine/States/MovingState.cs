using MortierFu.Shared;
using UnityEngine;
using UnityEngine.InputSystem;

public class MovingState : PlayerBaseState
{
    public MovingState(PlayerCharacter character, Animator animator) : base(character, animator)
    { }

    private float x, y, sprint;
    private bool _isSprinting;

    public override void OnEnter()
    {
        Logs.Log($"[Player {character.PlayerIndex}] Moving state entered");
        //animator?.CrossFade(MovingHash, k_crossFadeDuration);
    }
        
    public override void Update()
    {
        switch (character.Controller.sprintInput)
        {
            case 0 :    //WALK
                character.Controller.HandleMovementUpdate();
                break;
            case 1 :    //SPRINT
                character.Controller.HandleMovementUpdate(1.3f);
                break;
        }
        AnimationUpdate();
    }

    private void AnimationUpdate()
    {
        x = Mathf.Lerp(x, character.Controller.moveInput.x, Time.deltaTime * 12);
        y = Mathf.Lerp(y, character.Controller.moveInput.y, Time.deltaTime * 12);
        
        sprint = Mathf.Lerp(sprint, character.Controller.sprintInput, Time.deltaTime * 12);
        
        animator.SetFloat("x", x);
        animator.SetFloat("y", y);
        
        animator.SetFloat("speed", _isSprinting ? 1 : 0);
    }
    
        
    public override void OnExit()
    {
        Logs.Log($"[Player {character.PlayerIndex}] Moving state exited");
    }
}