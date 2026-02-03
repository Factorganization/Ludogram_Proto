using System;
using BillSimulation;
using Unity.Cinemachine;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

    /* PlayerController applies the supposed actions linked to the inputs on the player Prefab */
    public class PlayerController : MonoBehaviour
    {


        #region Fields
        [Header("Move Settings")]
        [Tooltip("Base movement speed when walking.")]
        [SerializeField] private float _walkSpeed = 30f;
        
        [Tooltip("Sensitivity of the mouse when looking around.")]

        [SerializeField] private float _sensitivity = 1f;
        
        [Tooltip("Height size of the Character")]
        [SerializeField] private float _characterSize = 2f;

        [Tooltip("Maximum vertical angle the player can look up or down.")]
        [Range(0, 90)]
        [SerializeField] private float _upDownLookRange = 80f;
        
        [Header("Jump Settings")]
        [SerializeField] private float jumpForce = 10;
        [SerializeField] private float airMultiplier = 0.4f;
        [SerializeField] private float groundDrag, airDrag;
        [SerializeField] private float maxAddedGravity, speedAddedGravity;
        private float currentAddedGravity;
        private RaycastHit slopeHit;
        [SerializeField] private float coyoteTime = 0.2f;
        private float currentFallTime;
        
        [Header("Camera effects")]
        [SerializeField] private Camera playerCam;
        [SerializeField] private float defaultFOV = 100;

        [Header("Interaction Settings")]
        [SerializeField] float interactionRange = 10;
        [SerializeField] private float interactionRadius = 0.25f; 
        [SerializeField] private LayerMask interactableLayer;
        
        [Header("References")]
        [SerializeField] private Transform feet;
        [SerializeField] private Transform headTransform;
        [SerializeField] PlayerInput _playerControls;
        [SerializeField] Rigidbody _playerRigidbody;
        
        private InputAction _movementAction;
        private InputAction _rotationAction;
        private InputAction _jumpAction;
        private InputAction _interactAction;

        private Vector3 moveDir, slopeMoveDir;
        private Vector2 moveVector, rotVector;
        private float _verticalRotation;
        private bool isMoving;
        private bool isGrounded;

        #endregion
        
        #region Entity Fields
        
        private Entity playerEntity;
        private EntityManager entityManager;
        
        #endregion

        #region Methods
        void Start()
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            
            SetInputReferences();
            
            playerCam.fieldOfView = defaultFOV;
            
            entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
            
            playerEntity = entityManager.CreateEntity(
                typeof(PlayerTag),
                typeof(PlayerMovement)
            );
        }
        
        void Update()
        {
            GroundCheck();
            
            ControlDrag();
            
            if (isGrounded && _playerRigidbody.linearVelocity.y < 0)
            {
                currentFallTime = 0;
            }

            if (entityManager.Exists(playerEntity))
            {
                float3 velocity = _playerRigidbody.linearVelocity / Time.deltaTime;

                entityManager.SetComponentData(playerEntity, new PlayerMovement
                {
                    Position = transform.position,
                    Velocity = velocity,
                    CollisionRadius = 0.5f
                });
            }
        }

        private void FixedUpdate()
        {
            HandleMovement();
            HandleRotation();
            
            if (!isGrounded)
            {
                currentAddedGravity = Mathf.SmoothStep(currentAddedGravity, maxAddedGravity, speedAddedGravity * Time.deltaTime);

                if (currentFallTime < coyoteTime + 0.1f)
                {
                    currentFallTime += Time.deltaTime;
                }
            }
            else if (currentAddedGravity != 0)
            {
                currentAddedGravity = 0;
            }
        }

        private void GroundCheck()
        {
            Color color = Color.red;
            
            isGrounded = Physics.Raycast(feet.position+ new Vector3(0,0.1f,0), Vector3.down, 0.5f);
            
            if(isGrounded) color = Color.green;
            
            Debug.DrawRay(feet.position, Vector3.down * 0.5f, color);
        }
        
        
        void Jump()
        {
            if (currentFallTime < coyoteTime && _playerRigidbody)
            {
                _playerRigidbody.linearVelocity = new Vector3(_playerRigidbody.linearVelocity.x, 0, _playerRigidbody.linearVelocity.z);
                _playerRigidbody.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);

                currentFallTime = coyoteTime + 1;
            }
        }
        
        private void TryInteract()
        {
            RaycastHit hit; 
            
            if (Physics.SphereCast(headTransform.position, interactionRadius, headTransform.forward, out hit, interactionRange, interactableLayer))
            {
                Debug.Log("Interacting with " + hit.collider.name);
                if (hit.collider.gameObject.TryGetComponent(out IInteractable interactable))
                {
                    interactable.Interact(transform);
                }
            }
        }


        private void HandleMovement()
        {
            
            slopeMoveDir = Vector3.ProjectOnPlane(moveDir, slopeHit.normal);

            Vector3 forward = playerCam.transform.forward;
            forward.y = 0;
        
            Vector3 right = playerCam.transform.right;
            forward.y = 0;
            
            moveDir = forward.normalized * moveVector.y + right * moveVector.x;

            _playerRigidbody.AddForce(Vector3.down * currentAddedGravity);
        
            if (isGrounded && !OnSlope())
            {
                _playerRigidbody.AddForce(moveDir.normalized * _walkSpeed, ForceMode.Acceleration);
            }
            else if (isGrounded && OnSlope())
            {
                _playerRigidbody.AddForce(slopeMoveDir.normalized * _walkSpeed, ForceMode.Acceleration);
            }
            else if (!isGrounded)
            {
                _playerRigidbody.AddForce(moveDir.normalized * _walkSpeed * airMultiplier, ForceMode.Acceleration);
            }
        }
        
        void ControlDrag()
        {
            if (OnSlope() && moveDir.magnitude <= 0.1f)
            {
                _playerRigidbody.linearDamping = 30;
            }
            else if (isGrounded)
            {
                _playerRigidbody.linearDamping = groundDrag;
            }
            else
            {
                _playerRigidbody.linearDamping = airDrag;
            }
        }
        
        private bool OnSlope()
        {
            if (Physics.Raycast(feet.position, Vector3.down, out slopeHit, 0.2f))
            {
                if (slopeHit.normal != Vector3.up)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            return false;
        }
        
        private void HandleRotation()
        {
            float mouseXRotation = rotVector.x * _sensitivity;
            float mouseYRotation = rotVector.y * _sensitivity;

            transform.Rotate(0, mouseXRotation, 0);
            
            _verticalRotation = Mathf.Clamp(_verticalRotation - mouseYRotation, -_upDownLookRange, _upDownLookRange);
            headTransform.localRotation = Quaternion.Euler(_verticalRotation, 0, 0);

        }
        
        public void SetInputReferences()
        {
            if (_playerControls == null)
                throw new MissingReferenceException("Please ensure that the player controls are assigned in the player input handler :)");
            
            _movementAction = _playerControls.actions["Movement"];
            _rotationAction = _playerControls.actions["Rotation"];
            _jumpAction = _playerControls.actions["Jump"];
            _interactAction = _playerControls.actions["Interaction"];

            SubscribeActionValuesToInputEvents();
        }
        
        private void SubscribeActionValuesToInputEvents()
        {
            _movementAction.performed += inputInfo => moveVector = inputInfo.ReadValue<Vector2>();
            _movementAction.canceled += inputInfo => moveVector  = Vector2.zero;

            _rotationAction.performed += inputInfo => rotVector = inputInfo.ReadValue<Vector2>();
            _rotationAction.canceled += inputInfo => rotVector = Vector2.zero;
            
            _jumpAction.started += inputInfo => Jump();

            _interactAction.performed += inputInfo => TryInteract();

        }

        
        #endregion
    }

