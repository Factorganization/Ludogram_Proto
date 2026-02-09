using CarScripts;
using UnityEngine;

public class SteeringWheel : MonoBehaviour, IInteractable
{
    [SerializeField] private CarController carController;
    
    public Transform GetTransform() => transform;

    public void Interact(IInteractable.InteractAction action, Transform interactor)
    {
        // Pour l'instant on ignore le type d'action et on route vers l'API legacy
        var playerCharacter = interactor.GetComponent<PlayerCharacter>();
        if (playerCharacter != null)
        {
            Interact(playerCharacter);
        }
    }

    public void Interact(PlayerCharacter interactor)
    {
        if (carController == null)
        {
            Debug.LogWarning("Car Controller not assigned on SteeringWheel.");
            return;
        }
        
        //interactor.Driving.BeginDriving();
    }
}
