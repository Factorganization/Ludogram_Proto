using System;
using CarScripts;
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
    [SerializeField] private CapsuleCollider playerCollider;
    [SerializeField] private Transform feet;
    [SerializeField] private Transform headTransform;
    [SerializeField] private Camera playerCam;

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

    [SerializeField] private DrivingInputs drivingDrivingInputs = new DrivingInputs();
    
    private RaycastHit _slopeHit;
    [SerializeField] private float horVelRagdoll = 12;
    [SerializeField] private float vertVelRagdoll = 20;
    private Vector2 _currentVelocity;
    private Vector2 _previousVelocity;
    private bool ragdoll;
    
    // TODO action handbrake 
    private InputAction handbrakeAction;
    private CarController carController;

    #endregion

    private void Awake()
    {
        CurrentSpeed = walkSpeed;
        
        drivingDrivingInputs ??= new DrivingInputs();
    }

    private void Start()
    {
        if (!playerCollider)
        {
            playerCollider = GetComponent<CapsuleCollider>();
        }
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
        
        drivingDrivingInputs.handbrakeInput = handbrakeAction.ReadValue<float>();
    }

    private void FixedUpdate()
    {
        HandleMovement();
        HandleRotation();
        ApplyExtraGravity();
        ResetSprintIfNotMoving();
        
        _currentVelocity = GetSpeed();
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
        _previousVelocity = GetSpeed();
        FreeFall();
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

        if (ragdoll)
        {
            SetRagdollMode(false);
        }
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
    
    public void OnRagdoll(InputValue inputValue)
    {
        if (inputValue.isPressed)
            SetRagdollMode(true);
    }

    #endregion
    
    # region INPUT CALLBACKS - DRIVING
    
    public void OnThrottle(InputValue inputValue)
    {
        drivingDrivingInputs.throttleInput = inputValue.Get<float>();
    }
    
    public void OnSteering(InputValue inputValue)
    {
        drivingDrivingInputs.steerInput = inputValue.Get<float>();
    }
    
    public void OnBrake(InputValue inputValue)
    {
        drivingDrivingInputs.brakeInput = inputValue.Get<float>();
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
        float mouseX = (rotVector.x * sensitivity) * Time.deltaTime;
        float mouseY = (rotVector.y * sensitivity) * Time.deltaTime;

        transform.Rotate(0, mouseX, 0);

        verticalRotation = Mathf.Clamp(
            verticalRotation - mouseY,
            -upDownLookRange,
            upDownLookRange);

        headTransform.localRotation = Quaternion.Euler(verticalRotation, 0, 0);
    }

    public Vector2 GetSpeed()
    {
        Vector3 playerVelocity = playerRigidbody.linearVelocity;

        Vector3 groundVelocity = Vector3.zero;
        
        if (Physics.Raycast(transform.position + Vector3.up * 0.1f, Vector3.down, out RaycastHit hit, 2f))
        {
            if (hit.rigidbody != null)
            {
                groundVelocity = hit.rigidbody.GetPointVelocity(hit.point);
            }
        }

        Vector3 relativeVelocity = playerVelocity - groundVelocity;
        
        float verticalSpeed = relativeVelocity.y;

        Vector3 horizontalVector = new Vector3(relativeVelocity.x, 0, relativeVelocity.z);
        float horizontalSpeed = horizontalVector.magnitude; 

        return new Vector2(horizontalSpeed, verticalSpeed);
    }
    
    private void FreeFall()
    {
        float horVelDiff = Mathf.Abs(_previousVelocity.x - _currentVelocity.x);
        float vertVelDiff = Mathf.Abs(_previousVelocity.y - _currentVelocity.y);
        
        if (horVelDiff > horVelRagdoll || vertVelDiff > vertVelRagdoll)
        {
            SetRagdollMode(true);
        }
    }

    private void SetRagdollMode(bool ragdollState)
    {
        ragdoll = ragdollState;
        if (ragdollState)
        {
            playerCollider.height = 0.001f;
        }
        else
        {
            playerCollider.height = 2f;
        }
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
                interactable.Interact(new PlayerCharacter());
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

    public void BeginDriving(CarController newCarController)
    {
        // Switch to driving action map and register vehicle input processor
        carController = newCarController;
        carController.AssignInputs(drivingDrivingInputs);
        SwitchActionMap("Driving");
        
        // TODO: CAMERA et tout le reste tia capté 
        Debug.Log($"Begin driving");
    }

    public void StopDriving()
    {
        // Switch back to player action map and unregister vehicle input processor
        carController.ClearInputs();
        carController = null;
        SwitchActionMap("Player");
        
        // TODO: CAMERA et tout le reste tia capté
    }

    #endregion
}
