using MortierFu.Shared;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.InputSystem;
using System;
using System.Collections.Generic;

[RequireComponent(typeof(PlayerInput), typeof(Rigidbody))]
public class PlayerCharacter : Actor
{
    public PlayerInput PlayerInput => GetCachedComponent<PlayerInput>();
    public int PlayerIndex => PlayerInput.playerIndex + 1; // PlayerIndex is 1-based for better readability in logs and UI.

    [field: SerializeField, Expandable]
    public SO_PlayerStats Playerstats { get; private set; }

    public ControllerComponent Controller { get; private set; }
    public DrivingComponent Driving { get; private set; }
    public InteractComponent Interact { get; private set; }
    
    private List<PlayerComponent> _components = new List<PlayerComponent>();
    
    private StateMachine _stateMachine;

    [Header("References")]
    [SerializeField] private Animator _animator;
    [SerializeField] internal UISwapper _uiSwapper;
    [Tooltip("Pivot transform for camera pitch (up/down look).")]
    [SerializeField] private Transform _headTransform;

    [SerializeField] private Transform _feetTransform;

    public Transform HeadTransform => _headTransform;
    public Transform FeetTransform => _feetTransform;

    // State flags for transitions (set these when entering/exiting states)
    public bool IsStunned { get; set; }
    public bool IsInteracting { get; set; }
    public bool IsDriving { get; set; }
    public bool IsFlying { get; set; }
    
    protected override void BeginPlay()
    {
        Controller = new ControllerComponent(this);
        Controller.Initialize();
        _components.Add(Controller);
        
        Driving = new DrivingComponent(this);
        Driving.Initialize();
        _components.Add(Driving);
        
        Interact = new InteractComponent(this);
        Interact.Initialize();
        _components.Add(Interact);
        
        InitStateMachine();
    }
    
    #region Unity Callbacks

    private void Update()
    {
        _stateMachine?.Update();
        if (_components == null) return;
        foreach (var component in _components)
        {
            component.Update();
        }
    }

    private void FixedUpdate()
    {
        _stateMachine?.FixedUpdate();
        if (_components == null) return;
        foreach (var component in _components)
        {
            component.FixedUpdate();
        }
    }

    private void LateUpdate()
    {
        _stateMachine?.LateUpdate();
    }

    private void OnDrawGizmos()
    {
        if (_components == null) return;
        foreach (var component in _components)
        {
            component.OnDrawGizmos();
        }
    }

    protected override void OnDrawGizmosSelected()
    {
        if (_components == null) return;
        foreach (var component in _components)
        {
            component.OnDrawGizmosSelected();
        }
    }

    #endregion


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
    
    public void SwitchInputMap(string mapName)
    {
        if (PlayerInput == null)
        {
            Logs.LogError("[PlayerCharacter]: PlayerInput component is missing.");
            return;
        }
        
        PlayerInput.SwitchCurrentActionMap(mapName);
        Logs.Log($"[PlayerCharacter]: Switched to input map '{mapName}'.");
    }

    #endregion

    #region State Machine

    private void InitStateMachine()
    {
        _stateMachine = new StateMachine();
        var movingState = new MovingState(this, _animator);
        var drivingState = new DrivingState(this, _animator);
        var flyingState = new FlyingState(this, _animator);
        var interactingState = new InteractingState(this, _animator);
        var stunState = new StunState(this, _animator);

        // --- Any() transitions: from any state when condition is true ---
        Any(stunState, new FuncPredicate(() => IsStunned));
        Any(drivingState, new FuncPredicate(() => IsDriving));
        Any(interactingState, new FuncPredicate(() => IsInteracting));
        Any(flyingState, new FuncPredicate(() => IsFlying));

        // --- At() transitions: return to Moving from specific states ---
        At(stunState, movingState, new FuncPredicate(() => !IsStunned));
        At(drivingState, movingState, new FuncPredicate(() => !IsDriving));
        At(interactingState, movingState, new FuncPredicate(() => !IsInteracting));
        At(flyingState, movingState, new FuncPredicate(() => !IsFlying));

        // Start in moving state
        _stateMachine.SetState(movingState);
    }
    private void At(IState from, IState to, IPredicate condition) =>
        _stateMachine.AddTransition(from, to, condition);
    private void Any(IState to, IPredicate condition) => _stateMachine.AddAnyTransition(to, condition);

    #endregion
    
}
