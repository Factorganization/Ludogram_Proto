using MortierFu.Shared;
using UnityEngine;
using UnityEngine.InputSystem;

public class ControllerComponent : PlayerComponent
{
    private InputAction _moveAction;
    private InputAction _lookAction;
    private InputAction _jumpAction;
    internal InputAction _sprintAction;
    
    private Rigidbody _rigidbody;
    public Rigidbody Rigidbody => _rigidbody;
    
    private float _verticalVelocity;
    private float _verticalRotation; 
    
    internal Vector2 moveInput, lookInput;
    internal bool jumpRequested;
    
    // Track if player is in a vehicle
    private bool _isInVehicle = false;
    private float _yawInVehicle = 0f; // Local yaw relative to vehicle

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
        
        _rigidbody = character.GetCachedComponent<Rigidbody>();

        Logs.Log("[PlayerControllerComponent] Initialized for " + character.name);
    }

    public void HandleMovementUpdate()
    {
        // Read input values
        moveInput = _moveAction.ReadValue<Vector2>();
        lookInput = _lookAction.ReadValue<Vector2>();

        if (_jumpAction.triggered) jumpRequested = true;
    }
    
    public void HandleLookUpdate()
    {
        if (character.IsStunned) return;
        
        float sensitivity = character.Playerstats.LookSensitivity;
        
        // Vertical look (pitch) - always local to head
        _verticalRotation -= lookInput.y * sensitivity;
        _verticalRotation = Mathf.Clamp(_verticalRotation, -90f, 90f);
        character.HeadTransform.localRotation = Quaternion.Euler(_verticalRotation, 0, 0);
        
        // Horizontal look (yaw) - depends on vehicle state
        if (_isInVehicle)
        {
            // When in vehicle, only rotate relative to vehicle (local yaw)
            _yawInVehicle += lookInput.x * sensitivity;
            
            // Apply rotation: vehicle's world rotation + local yaw
            float targetYaw = character.transform.parent.eulerAngles.y + _yawInVehicle;
            character.transform.rotation = Quaternion.Euler(0, targetYaw, 0);
        }
        else
        {
            // When not in vehicle, rotate normally in world space
            character.transform.Rotate(Vector3.up * lookInput.x * sensitivity);
        }
    }
    
    public void HandleFixedMovementUpdate(float speedMultiplier = 1)
    {
        if (_rigidbody == null) return;
        
        // Ground check
        GroundCheck(out bool isGrounded);
        
        // Apply movement
        Vector3 moveDirection = new Vector3(moveInput.x, 0, moveInput.y).normalized;
        Vector3 worldMoveDirection = character.transform.TransformDirection(moveDirection);
        Vector3 targetVelocity = worldMoveDirection * character.Playerstats.Speed * speedMultiplier;
        Vector3 velocityChange = targetVelocity - _rigidbody.linearVelocity;
        
        // Only change horizontal velocity, preserve vertical velocity
        velocityChange.y = 0;
        _rigidbody.AddForce(velocityChange, ForceMode.VelocityChange);
        
        // Handle jumping
        if (jumpRequested && isGrounded)
        {
            _rigidbody.AddForce(Vector3.up * character.Playerstats.JumpForce, ForceMode.Impulse);
            jumpRequested = false; 
        }
    }
    
    /// <summary>
    /// Call this when the player enters a vehicle
    /// </summary>
    public void EnterVehicle(Transform vehicleTransform)
    {
        _isInVehicle = true;
        
        // Calculate initial local yaw relative to vehicle
        float currentWorldYaw = character.transform.eulerAngles.y;
        float vehicleWorldYaw = vehicleTransform.eulerAngles.y;
        _yawInVehicle = Mathf.DeltaAngle(vehicleWorldYaw, currentWorldYaw);
        
        // Parent to vehicle
        character.transform.SetParent(vehicleTransform);
        
        Logs.Log($"[ControllerComponent] Entered vehicle. Local yaw: {_yawInVehicle}");
    }
    
    /// <summary>
    /// Call this when the player exits a vehicle
    /// </summary>
    public void ExitVehicle(Transform originalParent = null)
    {
        _isInVehicle = false;
        _yawInVehicle = 0f;
        
        // Unparent from vehicle
        character.transform.SetParent(originalParent);
        
        Logs.Log("[ControllerComponent] Exited vehicle");
    }
    
    public void ResetVelocity()
    {
        if (_rigidbody == null) return;
        _rigidbody.linearVelocity = Vector3.zero;
        _rigidbody.angularVelocity = Vector3.zero;
    }
    
    private void GroundCheck(out bool isGrounded)
    {
        Color color = Color.red;

        isGrounded = Physics.Raycast(character.FeetTransform.position + new Vector3(0, 0.1f, 0), Vector3.down, 0.5f);

        if(isGrounded) color = Color.green;

        Debug.DrawRay(character.FeetTransform.position, Vector3.down * 0.5f, color);
    }

    public override void Dispose()
    {
        // clear references
        _moveAction = null;
        _lookAction = null;
        _jumpAction = null;
        _rigidbody = null;
    }
}