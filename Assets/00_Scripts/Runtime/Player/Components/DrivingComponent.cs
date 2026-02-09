using CarScripts;
using MortierFu.Shared;
using UnityEngine.InputSystem;

public class DrivingComponent : PlayerComponent
{
    private InputAction _throttleAction;
    private InputAction _brakeAction;
    private InputAction _steerAction;
    private InputAction _exitVehicleAction;

    public DrivingInputs DrivingInputs = new DrivingInputs();
    
    private CarController currentCarController;
    
    
    public DrivingComponent(PlayerCharacter character) : base(character)
    {
    }

    public override void Initialize()
    {
        character.FindInputAction("Throttle", out _throttleAction);
        character.FindInputAction("Brake", out _brakeAction);
        character.FindInputAction("Steer", out _steerAction);
        character.FindInputAction("ExitVehicle", out _exitVehicleAction);

        if (_throttleAction == null || _brakeAction == null || _steerAction == null || _exitVehicleAction == null)
        {
            Logs.LogError("[DrivingComponent] One or more driving InputActions not found. Please check the input configuration.");
        }
    }

    public void HandleDrivingUpdate()
    {
        if (currentCarController == null) return;

        DrivingInputs.throttleInput = _throttleAction.ReadValue<float>();
        DrivingInputs.brakeInput = _brakeAction.ReadValue<float>();
        DrivingInputs.steerInput = _steerAction.ReadValue<float>();
        
        if (_exitVehicleAction.triggered)
        {
            EndDriving();
        }
    }

    public void BeginDriving(CarController carController)
    {
            if (carController == null)
            {
                Logs.LogWarning("[DrivingComponent] CarController is null. Cannot begin driving.");
                return;
            }
            
            carController.AssignInputs(DrivingInputs);
            currentCarController = carController;
            character.IsDriving = true;
            
    }
    
    private void EndDriving()
    {
        currentCarController.ClearInputs();
        currentCarController = null;
        character.IsDriving = false;
    }
}