using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class CarInputHandler : MonoBehaviour
{
    private InputAction _movementAction;
    private InputAction _jumpAction;

    private Vector2 moveVector;
    [SerializeField] InputActionAsset _playerControls;
    [SerializeField] private SCC_InputProcessor inputProcessor;
    private Inputs inputs = new Inputs();

    private void Start()
    {
        SubscribeActionValuesToInputEvents();
    }

    private void Update()
    {
        Acceleration();
        Turn();
        
        inputProcessor.OverrideInputs(inputs);
    }

    private void Acceleration()
    {
        inputs.throttleInput = moveVector.y;
    }

    private void Turn()
    {
        inputs.steerInput = moveVector.x;
    }

    private void Brake(bool state)
    {
        if (state)
        {
            inputs.handbrakeInput = 1;
        }
        else
        {
            inputs.handbrakeInput = 0;
        }
        
    }
    
    private void SubscribeActionValuesToInputEvents()
    {
        if (_playerControls == null)
            throw new MissingReferenceException("Please ensure that the player controls are assigned in the player input handler :)");
        InputActionMap mapReference = _playerControls.FindActionMap("Player");

        _movementAction = mapReference.FindAction("Movement");
        _jumpAction = mapReference.FindAction("Jump");
        
        _movementAction.performed += inputInfo => moveVector = inputInfo.ReadValue<Vector2>();
        _movementAction.canceled += inputInfo => moveVector  = Vector2.zero;
            
        _jumpAction.started += inputInfo => Brake(true);
        _jumpAction.canceled += inputInfo => Brake(false);

        
        //_interactAction.performed += inputInfo => ;
    }
}
