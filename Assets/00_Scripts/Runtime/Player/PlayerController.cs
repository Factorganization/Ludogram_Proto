using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    #region Fields

    [Header("Move Settings")]
    public float walkSpeed = 30f;
    public float sprintSpeed = 70f;

    [SerializeField] private float sensitivity = 1f;
    [SerializeField] private float upDownLookRange = 80f;

    [Header("Jump Settings")]
    [SerializeField] private float jumpForce = 10;
    [SerializeField] private float airMultiplier = 0.4f;
    [SerializeField] private float groundDrag, airDrag;
    [SerializeField] private float maxAddedGravity, speedAddedGravity;
    [SerializeField] private float coyoteTime = 0.2f;

    [Header("References")]
    [SerializeField] private PlayerInput playerInput;
    [SerializeField] private Rigidbody playerRigidbody;
    [SerializeField] private Transform feet;
    [SerializeField] private Transform headTransform;
    [SerializeField] private Camera playerCam;

    [Header("Driving")] 
    [SerializeField] private SCC_InputProcessor vehicleInputProcessor;

    [Header("Debug")]
    public float CurrentSpeed { get; private set; }
    [SerializeField] private Vector2 rotVector;
    public Vector2 MoveVector { get; private set; }
    [SerializeField] private float verticalRotation;
    [SerializeField] public bool isGrounded;
    [SerializeField] private float currentAddedGravity;
    [SerializeField] private float currentFallTime;
    [SerializeField] private Vector3 moveDir;
    [SerializeField] private Vector3 slopeMoveDir;
    [SerializeField] private Transform carrierTransform;

    [SerializeField] private SCC_Inputs drivingInputs = new SCC_Inputs();
    
    private RaycastHit _slopeHit;
    
    // TODO action handbrake 
    private InputAction handbrakeAction;
    

    #endregion

    private void Awake()
    {
        CurrentSpeed = walkSpeed;
        
        drivingInputs ??= new SCC_Inputs();
    }

    private void OnEnable()
    {
        playerInput.ActivateInput();
        
        handbrakeAction = playerInput.actions["Handbrake"];
        
        SwitchActionMap("Player");
    }

    private void OnDisable()
    {
        playerInput.DeactivateInput();
    }

    private void Update()
    {
        GroundCheck();
        ControlDrag();

        if (isGrounded && playerRigidbody.linearVelocity.y < 0)
            currentFallTime = 0;
        
        // Driving input override
        
        drivingInputs.handbrakeInput = handbrakeAction.ReadValue<float>();
        vehicleInputProcessor?.OverrideInputs(drivingInputs);
    }

    private void FixedUpdate()
    {
        HandleMovement();
        HandleRotation();
        ApplyExtraGravity();
        ResetSprintIfNotMoving();
        
    }
    
    private void OnCollisionEnter(Collision other)
    {
        if (other.collider.CompareTag("Car"))
        {
            // Hard codé sa mère pardon 
            OnEnterMovingObject(SCC_InputProcessor.Instance?.transform);
        }
        
        if (other.collider.CompareTag("MovableObject"))
        {
            OnEnterMovingObject(other.transform);
        }
    }

    private void OnCollisionStay(Collision other)
    {
        // Pardon maman
        if (other.collider.CompareTag("Car"))
        {
            // Hard codé sa mère pardon 
            OnEnterMovingObject(SCC_InputProcessor.Instance?.transform);
        }
        
        if (other.collider.CompareTag("MovableObject"))
        {
            OnEnterMovingObject(other.transform);
        }
    }

    private void OnCollisionExit(Collision other)
    {
        if (other.collider.CompareTag("MovableObject") || other.collider.CompareTag("Car"))
        {
            OnExitMovingObject();
        }
    }

    #region INPUT CALLBACKS - PLAYER 

    public void OnMovement(InputValue inputValue)
    {
        MoveVector = inputValue.Get<Vector2>();
    }

    public void OnRotation(InputValue inputValue)
    {
        rotVector = inputValue.Get<Vector2>();
    }

    public void OnJump(InputValue inputValue)
    {
        if (inputValue.isPressed)
            Jump();
    }

    public void OnSprint(InputValue inputValue)
    {
        // C'est un hold parce que c'est mieux
        CurrentSpeed = inputValue.isPressed ? sprintSpeed : walkSpeed;
    }

    public void OnInteraction(InputValue inputValue)
    {
        if (inputValue.isPressed)
            TryInteract();
    }

    #endregion
    
    # region INPUT CALLBACKS - DRIVING
    
    public void OnThrottle(InputValue inputValue)
    {
        drivingInputs.throttleInput = inputValue.Get<float>();
    }
    
    public void OnSteering(InputValue inputValue)
    {
        drivingInputs.steerInput = inputValue.Get<float>();
    }
    
    public void OnBrake(InputValue inputValue)
    {
        drivingInputs.brakeInput = inputValue.Get<float>();
    }

    
    public void OnExitVehicle(InputValue inputValue)
    {
        if (inputValue.isPressed)
            StopDriving();
    }
    
    #endregion

    #region Movement

    private void HandleMovement()
    {
        Vector3 forward = playerCam.transform.forward;
        Vector3 right = playerCam.transform.right;

        forward.y = 0;
        right.y = 0;

        moveDir = forward.normalized * MoveVector.y +
                  right.normalized * MoveVector.x;

        slopeMoveDir = Vector3.ProjectOnPlane(moveDir, _slopeHit.normal);

        playerRigidbody.AddForce(Vector3.down * currentAddedGravity);

        if (isGrounded)
        {
            Vector3 dir = OnSlope() ? slopeMoveDir : moveDir;
            playerRigidbody.AddForce(dir.normalized * CurrentSpeed, ForceMode.Acceleration);
        }
        else
        {
            playerRigidbody.AddForce(moveDir.normalized * (CurrentSpeed * airMultiplier),
                ForceMode.Acceleration);
        }
    }

    private void HandleRotation()
    {
        float mouseX = rotVector.x * sensitivity;
        float mouseY = rotVector.y * sensitivity;

        transform.Rotate(0, mouseX, 0);

        verticalRotation = Mathf.Clamp(
            verticalRotation - mouseY,
            -upDownLookRange,
            upDownLookRange);

        headTransform.localRotation = Quaternion.Euler(verticalRotation, 0, 0);
    }

    #endregion

    #region Physics

    private void GroundCheck()
    {
        isGrounded = Physics.Raycast(feet.position + Vector3.up * 0.1f, Vector3.down, 0.5f);
    }

    private bool OnSlope()
    {
        if (Physics.Raycast(feet.position, Vector3.down, out _slopeHit, 0.2f))
            return _slopeHit.normal != Vector3.up;

        return false;
    }

    private void ControlDrag()
    {
        if (OnSlope() && moveDir.magnitude <= 0.1f)
            playerRigidbody.linearDamping = 30;
        else if (isGrounded)
            playerRigidbody.linearDamping = groundDrag;
        else
            playerRigidbody.linearDamping = airDrag;
    }

    private void ApplyExtraGravity()
    {
        if (!isGrounded)
        {
            currentAddedGravity = Mathf.SmoothStep(
                currentAddedGravity,
                maxAddedGravity,
                speedAddedGravity * Time.deltaTime);

            if (currentFallTime < coyoteTime + 0.1f)
                currentFallTime += Time.deltaTime;
        }
        else
        {
            currentAddedGravity = 0;
        }
    }

    #endregion

    #region Actions

    private void Jump()
    {
        if (currentFallTime >= coyoteTime) return;

        playerRigidbody.linearVelocity =
            new Vector3(playerRigidbody.linearVelocity.x, 0, playerRigidbody.linearVelocity.z);

        playerRigidbody.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);

        currentFallTime = coyoteTime + 1;
    }

    private void ResetSprintIfNotMoving()
    {
        if (playerRigidbody.linearVelocity.magnitude < 1f)
            CurrentSpeed = walkSpeed;
    }

    private void TryInteract()
    {
        if (!headTransform) return;

        if (Physics.SphereCast(
                headTransform.position,
                0.25f,
                headTransform.forward,
                out var hit,
                10f))
        {
            if (hit.collider.TryGetComponent(out IInteractable interactable))
                interactable.Interact(this);
        }
    }
    
    private void OnEnterMovingObject(Transform objectTransform)
    {
        // Set itself as child of the moving object
        carrierTransform = objectTransform;
        transform.SetParent(carrierTransform);
    }
    
    private void OnExitMovingObject()
    {
        // Unset itself as child
        carrierTransform = null;
        transform.SetParent(null);
    }
    
    private void SwitchActionMap(string map)
    {
        playerInput.SwitchCurrentActionMap(map);
    }

    public void BeginDriving(SCC_InputProcessor vehicleInputProcessor)
    {
        // Switch to driving action map and register vehicle input processor
        this.vehicleInputProcessor = vehicleInputProcessor;
        SwitchActionMap("Driving");
        
        // TODO: CAMERA et tout le reste tia capté 
    }

    public void StopDriving()
    {
        // Switch back to player action map and unregister vehicle input processor
        vehicleInputProcessor = null;
        SwitchActionMap("Player");
        
        // TODO: CAMERA et tout le reste tia capté
    }

    #endregion
}
