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
        switch (_isSprinting)
        {
            case false :    //WALK
                character.Controller.HandleMovementUpdate();
                break;
            case true :    //SPRINT
                character.Controller.HandleMovementUpdate(1.7f);
                break;
        }
        AnimationUpdate();
    }

    private void AnimationUpdate()
    {
        x = Mathf.Lerp(x, character.Controller.moveInput.x, Time.deltaTime * 12);
        y = Mathf.Lerp(y, character.Controller.moveInput.y, Time.deltaTime * 12);
        
        SprintHandler();
        sprint = Mathf.Lerp(sprint, _isSprinting ? 1 : 0, Time.deltaTime * 12);
        
        animator.SetFloat("x", x);
        animator.SetFloat("y", y);
        
        animator.SetFloat("speed", sprint);
    }

    private void SprintHandler()
    {
        if (character.Controller._sprintAction.WasPressedThisFrame())
        {
            _isSprinting = !_isSprinting;
        }

        if (_isSprinting && character.Controller._controller.velocity.magnitude < 0.2f)
        {
            _isSprinting = false;
        }
    }
    
        
    public override void OnExit()
    {
        Logs.Log($"[Player {character.PlayerIndex}] Moving state exited");
    }
}