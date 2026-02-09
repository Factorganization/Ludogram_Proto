using MortierFu.Shared;
using UnityEngine;
using UnityEngine.InputSystem;

public class ControllerComponent : PlayerComponent
{
    private InputAction _moveAction;
    private InputAction _lookAction;
    private InputAction _jumpAction;

    private CharacterController _controller;
    private float _verticalVelocity;
    private float _verticalRotation; 
    
    // Tunables for a more "classic" FPS feel
    private const float ExtraFallGravityMultiplier = 2.0f;   // Makes falling snappier than going up
    private const float AirControlMultiplier       = 0.9f;   // Slight reduction of control while in air

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

        Vector2 moveInput = _moveAction != null ? _moveAction.ReadValue<Vector2>() : Vector2.zero;
        Vector2 lookInput = _lookAction != null ? _lookAction.ReadValue<Vector2>() : Vector2.zero;
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
        move *= character.Playerstats.Speed;

        // Slightly reduce control in air for a more grounded feeling
        if (!_controller.isGrounded)
        {
            move *= AirControlMultiplier;
        }

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
            // Apply stronger gravity when falling for less "moon-like" jumps
            float gravity = character.Playerstats.Gravity;

            if (_verticalVelocity > 0f)
            {
                // Going up: normal gravity
                _verticalVelocity += gravity * Time.deltaTime;
            }
            else
            {
                // Falling: extra gravity for a snappier feel
                _verticalVelocity += gravity * ExtraFallGravityMultiplier * Time.deltaTime;
            }
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
