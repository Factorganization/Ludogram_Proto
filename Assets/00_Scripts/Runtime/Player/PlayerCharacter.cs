using MortierFu.Shared;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.InputSystem;
using System;

[RequireComponent(typeof(PlayerInput), typeof(CharacterController), typeof(Rigidbody))]
public class PlayerCharacter : Actor
{
    public PlayerInput PlayerInput => GetCachedComponent<PlayerInput>();
    public int PlayerIndex => PlayerInput.playerIndex + 1; // PlayerIndex is 1-based for better readability in logs and UI.

    [field: SerializeField, Expandable]
    public SO_PlayerStats Playerstats { get; private set; }

    public ControllerComponent Controller { get; private set; }
    public DrivingComponent Driving { get; private set; }
    private StateMachine _stateMachine;

    [Header("References")]
    [SerializeField] private Animator _animator;

    public bool IsStunned { get; private set; }
    
    protected override void BeginPlay()
    {
        Controller = new ControllerComponent(this);
        Controller.Initialize();
        
        Driving = new DrivingComponent(this);
        Driving.Initialize();
        
        InitStateMachine();
    }

    private void Update()
    {
        _stateMachine.Update();
    }


    #region Inputs

    public void FindInputAction(string actionName, out InputAction action)
    {
#if UNITY_EDITOR
        bool isEditor = true;
#else
            bool isEditor = false;
#endif
        action = PlayerInput.actions.FindAction(actionName, isEditor);
        if (action == null)
        {
            Logs.LogError($"[PlayerCharacter]: Input Action '{actionName}' not found in PlayerInput actions.");
        }
    }

    #endregion

    #region State Machine

    private void InitStateMachine()
    {
        _stateMachine = new StateMachine();
        var movingState = new MovingState(this, _animator);

        // Create other states
        var drivingState = new DrivingState(this, _animator);
        var flyingState = new FlyingState(this, _animator);
        var interactingState = new InteractingState(this, _animator);
        var stunState = new StunState(this, _animator);

        // Helper to create IPredicate from a Func<bool>
        // (private nested class below)
        // Any -> Interacting when Interact action is triggered
        

        At(movingState, stunState, new FuncPredicate(() => IsStunned));
        At(stunState, movingState, new FuncPredicate(() => !IsStunned));
        
        
        

        // Start in moving state
        _stateMachine.SetState(movingState);
    }
    private void At(IState from, IState to, IPredicate condition) =>
        _stateMachine.AddTransition(from, to, condition);
    private void Any(IState to, IPredicate condition) => _stateMachine.AddAnyTransition(to, condition);

    #endregion
    
}
