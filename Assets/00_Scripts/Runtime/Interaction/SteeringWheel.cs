using CarScripts;
using MortierFu.Shared;
using UnityEngine;

public class SteeringWheel : MonoBehaviour, IInteractable
{
    [SerializeField] private CarController carController;
    
    public Transform GetTransform() => transform;

    public void Interact(IInteractable.InteractAction action, Transform interactor)
    {
        // Pour l'instant on ignore le type d'action et on route vers l'API legacy
        var playerCharacter = interactor.GetComponent<PlayerCharacter>();
        if (playerCharacter)
        {
            Interact(playerCharacter);
        }
    }

    public void Interact(PlayerCharacter interactor)
    {
        if (!carController)
        {
            Logs.LogWarning("Car Controller not assigned on SteeringWheel.");
            return;
        }
        
        interactor.Driving.BeginDriving(carController);
    }
}
