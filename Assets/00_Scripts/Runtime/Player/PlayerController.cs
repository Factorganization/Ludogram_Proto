using MortierFu.Shared;
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

    private Vector2 _rotVector;
    public Vector2 MoveVector { get; private set; }
    
    private float _verticalRotation;

    public float CurrentSpeed { get; private set; }

    public bool isGrounded;
    private float _currentAddedGravity;
    private float _currentFallTime;

    private RaycastHit _slopeHit;
    private Vector3 _moveDir;
    private Vector3 _slopeMoveDir;

    private Transform _parentTransform;

    #endregion

    private void Awake()
    {
        _parentTransform = transform.root;
        CurrentSpeed = walkSpeed;
        
        
    }

    private void OnEnable()
    {
        playerInput.ActivateInput();
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
            _currentFallTime = 0;
    }

    private void FixedUpdate()
    {
        HandleMovement();
        HandleRotation();
        ApplyExtraGravity();
        ResetSprintIfNotMoving();
    }

    #region INPUT CALLBACKS

    public void OnMovement(InputValue inputValue)
    {
        Logs.LogError("OnMovement called");
        MoveVector = inputValue.Get<Vector2>();
    }

    public void OnRotation(InputValue inputValue)
    {
        _rotVector = inputValue.Get<Vector2>();
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

    #region ACTION MAP

    private void SwitchActionMap(string map)
    {
        playerInput.SwitchCurrentActionMap(map);
    }

    public void EnterVehicle()
    {
        SwitchActionMap("Driving");
    }

    public void ExitVehicle()
    {
        SwitchActionMap("Player");
    }

    #endregion

    #region Movement

    private void HandleMovement()
    {
        Vector3 forward = playerCam.transform.forward;
        Vector3 right = playerCam.transform.right;

        forward.y = 0;
        right.y = 0;

        _moveDir = forward.normalized * MoveVector.y +
                  right.normalized * MoveVector.x;

        _slopeMoveDir = Vector3.ProjectOnPlane(_moveDir, _slopeHit.normal);

        playerRigidbody.AddForce(Vector3.down * _currentAddedGravity);

        if (isGrounded)
        {
            Vector3 dir = OnSlope() ? _slopeMoveDir : _moveDir;
            playerRigidbody.AddForce(dir.normalized * CurrentSpeed, ForceMode.Acceleration);
        }
        else
        {
            playerRigidbody.AddForce(_moveDir.normalized * (CurrentSpeed * airMultiplier),
                ForceMode.Acceleration);
        }
    }

    private void HandleRotation()
    {
        float mouseX = _rotVector.x * sensitivity;
        float mouseY = _rotVector.y * sensitivity;

        transform.Rotate(0, mouseX, 0);

        _verticalRotation = Mathf.Clamp(
            _verticalRotation - mouseY,
            -upDownLookRange,
            upDownLookRange);

        headTransform.localRotation = Quaternion.Euler(_verticalRotation, 0, 0);
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
        if (OnSlope() && _moveDir.magnitude <= 0.1f)
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
            _currentAddedGravity = Mathf.SmoothStep(
                _currentAddedGravity,
                maxAddedGravity,
                speedAddedGravity * Time.deltaTime);

            if (_currentFallTime < coyoteTime + 0.1f)
                _currentFallTime += Time.deltaTime;
        }
        else
        {
            _currentAddedGravity = 0;
        }
    }

    #endregion

    #region Actions

    private void Jump()
    {
        if (_currentFallTime >= coyoteTime) return;

        playerRigidbody.linearVelocity =
            new Vector3(playerRigidbody.linearVelocity.x, 0, playerRigidbody.linearVelocity.z);

        playerRigidbody.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);

        _currentFallTime = coyoteTime + 1;
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
                interactable.Interact(transform);
        }
    }

    #endregion
}
