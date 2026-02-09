using MortierFu.Shared;
using UnityEngine;

public class InteractingState : PlayerBaseState
{
    public InteractingState(PlayerCharacter character, Animator animator) : base(character, animator)
    { }

    public override void OnEnter()
    {
        Logs.Log($"[Player {character.PlayerIndex}] Interacting state entered");
        //animator?.CrossFade(InteractingHash, k_crossFadeDuration);
    }
        
    public override void Update()
    {
        // TODO: Handle interaction logic here (e.g., checking for interaction completion)
    }
    
        
    public override void OnExit()
    {
        Logs.Log($"[Player {character.PlayerIndex}] Interacting state exited");
    }
}