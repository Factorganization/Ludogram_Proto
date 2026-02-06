using MortierFu.Shared;
using UnityEngine;
using UnityEngine.InputSystem;

public class ControllerComponent : CharacterComponent
{
    private InputAction _moveAction;
    private InputAction _lookAction;
    private InputAction _jumpAction;

    private CharacterController _controller;
    private float _verticalVelocity;
    

    public ControllerComponent(PlayerCharacter character) : base(character)
    {
        if (character == null) return;
    }

    public override void Initialize()
    {
        character.FindInputAction("Move", out _moveAction);
        character.FindInputAction("Look", out _lookAction);
        character.FindInputAction("Jump", out _jumpAction);

        _controller = character.GetCachedComponent<CharacterController>();
        if (_controller == null)
        {
            Logs.LogError("[PlayerControllerComponent] CharacterController not found on character.");
        }

        Logs.Log("[PlayerControllerComponent] Initialized for " + character.name);
    }

    public void HandleMovementUpdate()
    {
        if (character == null || _controller == null) return;
        if (!PlayerCharacter.AllowGameplayActions) return;

        Vector2 moveInput = _moveAction != null ? _moveAction.ReadValue<Vector2>() : Vector2.zero;
        Vector2 lookInput = _lookAction != null ? _lookAction.ReadValue<Vector2>() : Vector2.zero;
        bool jumpRequested = _jumpAction != null && _jumpAction.triggered;

        // Yaw rotation from look x
        if (lookInput.x != 0f)
        {
            float yaw = lookInput.x * character.Playerstats.LookSensitivity;
            character.transform.Rotate(0f, yaw, 0f);
        }

        // Movement in local XZ
        Vector3 right = character.transform.right;
        Vector3 forward = Vector3.ProjectOnPlane(character.transform.forward, Vector3.up).normalized;
        Vector3 move = forward * moveInput.y + right * moveInput.x;
        move *= character.Playerstats.Speed;

        // Gravity & Jump
        if (_controller.isGrounded)
        {
            // small negative so controller stays grounded
            _verticalVelocity = -0.5f;
            if (jumpRequested)
            {
                _verticalVelocity = Mathf.Sqrt(character.Playerstats.JumpForce * -2f * character.Playerstats.Gravity);
            }
        }
        else
        {
            _verticalVelocity += character.Playerstats.Gravity * Time.deltaTime;
        }

        move.y = _verticalVelocity;

        // Move the character
        _controller.Move(move * Time.deltaTime);
    }

    public override void Dispose()
    {
        // clear references
        _moveAction = null;
        _lookAction = null;
        _jumpAction = null;
        _controller = null;
    }
}
