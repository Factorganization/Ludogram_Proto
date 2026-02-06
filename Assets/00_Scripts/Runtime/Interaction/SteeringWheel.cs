using CarScripts;
using UnityEngine;

public class SteeringWheel : MonoBehaviour, IInteractable
{
    [SerializeField] private CarController carController;
    public void Interact(PlayerController interactor)
    {
        if (carController == null)
        {
            Debug.LogWarning("Car Controller not assigned on SteeringWheel.");
            return;
        }
        
        interactor.BeginDriving(carController);
    }
}
