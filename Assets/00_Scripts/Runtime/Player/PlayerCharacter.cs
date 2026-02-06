using MortierFu.Shared;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput), typeof(CharacterController), typeof(Rigidbody))]
public class PlayerCharacter : Actor
{
    public static bool AllowGameplayActions { get; set; }

    public PlayerInput PlayerInput => GetCachedComponent<PlayerInput>();
    public int PlayerIndex => PlayerInput.playerIndex + 1; // PlayerIndex is 1-based for better readability in logs and UI.
    
    [field: SerializeField, Expandable]
    public SO_PlayerStats Playerstats { get; private set; }
    
    public ControllerComponent Controller { get; private set; }
    private StateMachine _stateMachine;
    private MovingState _movingState;
    
    private Animator _animator;

    protected override void BeginPlay()
    {
        Controller = new ControllerComponent(this);
        _animator = GetCachedComponent<Animator>();
        
        Controller.Initialize();
        
        InitStateMachine();
    }

    private void InitStateMachine()
    {
        _stateMachine = new StateMachine();
        _movingState = new MovingState(this, _animator);
        
        // Define transitions
        
        
        
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
    
    private void At(IState from, IState to, IPredicate condition) =>
        _stateMachine.AddTransition(from, to, condition);

    private void Any(IState to, IPredicate condition) => _stateMachine.AddAnyTransition(to, condition);
    
    #endregion
}
