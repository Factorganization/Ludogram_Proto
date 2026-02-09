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
    

    private const float ExtraFallGravityMultiplier = 2.0f;   
    private const float AirControlMultiplier       = 0.9f;   
    private const float LookInputThresholdSqr      = 0.01f * 0.01f;

    private bool IsCurrentDeviceMouse
    {
        get
        {
            var playerInput = character?.PlayerInput;
            if (playerInput == null) return false;

            var scheme = playerInput.currentControlScheme;
            return scheme == "KeyboardMouse" || scheme == "Keyboard&Mouse";
        }
    }

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
        
        if (lookInput.sqrMagnitude >= LookInputThresholdSqr && character.HeadTransform != null)
        {
            float deltaTimeMultiplier = IsCurrentDeviceMouse ? 1.0f : Time.deltaTime;
            float rotationSpeed = sensitivity;

            // Pitch (vertical look)
            _verticalRotation += lookInput.y * rotationSpeed * deltaTimeMultiplier;
            _verticalRotation = ClampAngle(_verticalRotation, -upDownRange, upDownRange);
            character.HeadTransform.localRotation = Quaternion.Euler(-_verticalRotation, 0f, 0f);

            // Yaw (horizontal look) - rotation du corps
            float yaw = lookInput.x * rotationSpeed * deltaTimeMultiplier;
            character.transform.Rotate(Vector3.up * yaw);
        }

        // Movement in local XZ
        Vector3 right = character.transform.right;
        Vector3 forward = Vector3.ProjectOnPlane(character.transform.forward, Vector3.up).normalized;
        Vector3 move = forward * moveInput.y + right * moveInput.x;
        move *= character.Playerstats.Speed;
        
        if (!_controller.isGrounded)
        {
            move *= AirControlMultiplier;
        }
        
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
            float gravity = character.Playerstats.Gravity;

            if (_verticalVelocity > 0f)
            {
                // Going up: normal gravity
                _verticalVelocity += gravity * Time.deltaTime;
            }
            else
            {
                // Falling: extra gravity
                _verticalVelocity += gravity * ExtraFallGravityMultiplier * Time.deltaTime;
            }
        }

        move.y = _verticalVelocity;

        _controller.Move(move * Time.deltaTime);
    }
    
    private static float ClampAngle(float angle, float min, float max)
    {
        if (angle < -360f) angle += 360f;
        if (angle > 360f) angle -= 360f;
        return Mathf.Clamp(angle, min, max);
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
