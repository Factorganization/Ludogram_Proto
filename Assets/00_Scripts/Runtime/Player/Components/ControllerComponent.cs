using MortierFu.Shared;
using UnityEngine;
using UnityEngine.InputSystem;

public class ControllerComponent : PlayerComponent
{
    private InputAction _moveAction;
    private InputAction _lookAction;
    private InputAction _jumpAction;
    internal InputAction _sprintAction;

    internal CharacterController _controller;
    private float _verticalVelocity;
    private float _verticalRotation; 
    
    internal Vector2 moveInput, lookInput;

    public ControllerComponent(PlayerCharacter character) : base(character)
    {
        if (character == null) return;
    }

    public override void Initialize()
    {
        character.FindInputAction("Move", out _moveAction);
        character.FindInputAction("Look", out _lookAction);
        character.FindInputAction("Jump", out _jumpAction);
        character.FindInputAction("Sprint", out _sprintAction);

        _controller = character.GetCachedComponent<CharacterController>();
        if (_controller == null)
        {
            Logs.LogError("[PlayerControllerComponent] CharacterController not found on character.");
        }

        Logs.Log("[PlayerControllerComponent] Initialized for " + character.name);
    }

    public void HandleMovementUpdate(float speedMultiplier = 1)
    {
        if (character == null || _controller == null) return;

        moveInput = _moveAction != null ? _moveAction.ReadValue<Vector2>() : Vector2.zero;
        lookInput = _lookAction != null ? _lookAction.ReadValue<Vector2>() : Vector2.zero;
        bool jumpRequested = _jumpAction != null && _jumpAction.triggered;

        float sensitivity = character.Playerstats.LookSensitivity;
        float upDownRange = character.Playerstats.UpDownLookRange;

        // Yaw (horizontal look) - rotates character body
        if (lookInput.x != 0f)
        {
            float yaw = lookInput.x * sensitivity;
            character.transform.Rotate(0f, yaw, 0f);
        }

        // Pitch (vertical look) - rotates head/camera pivot
        if (lookInput.y != 0f && character.HeadTransform != null)
        {
            _verticalRotation -= lookInput.y * sensitivity;
            _verticalRotation = Mathf.Clamp(_verticalRotation, -upDownRange, upDownRange);
            character.HeadTransform.localRotation = Quaternion.Euler(_verticalRotation, 0f, 0f);
        }

        // Movement in local XZ
        Vector3 right = character.transform.right;
        Vector3 forward = Vector3.ProjectOnPlane(character.transform.forward, Vector3.up).normalized;
        Vector3 move = forward * moveInput.y + right * moveInput.x;
        move *= (character.Playerstats.Speed * speedMultiplier);

        // Gravity & Jump - JumpHeight is in meters, formula: v = sqrt(2 * g * h)
        if (_controller.isGrounded)
        {
            _verticalVelocity = -0.5f;
            if (jumpRequested)
            {
                float g = Mathf.Abs(character.Playerstats.Gravity);
                _verticalVelocity = Mathf.Sqrt(2f * g * character.Playerstats.JumpHeight);
            }
        }
        else
        {
            _verticalVelocity += character.Playerstats.Gravity * Time.deltaTime;
        }

        move.y = _verticalVelocity;

        _controller.Move(move * Time.deltaTime);
    }
    
    public void ResetVelocity()
    {
        // Stop all movement immediately by moving the controller with zero velocity
        if (_controller != null)
        {
            _controller.Move(Vector3.zero);
        }
        _verticalVelocity = 0f;
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
