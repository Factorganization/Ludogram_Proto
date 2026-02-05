using UnityEngine;

public class SteeringWheel : MonoBehaviour, IInteractable
{
    [SerializeField] private SCC_InputProcessor _inputProcessor;
    
    public void Interact(PlayerController interactor)
    {
        if (_inputProcessor == null)
        {
            Debug.LogWarning("Input Processor not assigned on SteeringWheel.");
            return;
        }
        
        interactor.BeginDriving(_inputProcessor);
    }
}
