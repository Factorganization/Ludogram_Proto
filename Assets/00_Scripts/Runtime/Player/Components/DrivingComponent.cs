using UnityEngine.InputSystem;

public class DrivingComponent : PlayerComponent
{
    public InputAction ThrottleAction { get; private set; }
    public InputAction BrakeAction { get; private set; }
    public InputAction SteerAction { get; private set; }

    public DrivingInputs DrivingDrivingInputs = new DrivingInputs();
    
    
    public DrivingComponent(PlayerCharacter character) : base(character)
    {
    }

    public void HandleDrivingUpdate()
    {
        
    }
    
}