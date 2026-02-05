using UnityEngine;

public class Ladder : MonoBehaviour, IInteractable
{
    [SerializeField] private float power = 1000;
    public void Interact(PlayerController interactor)
    {
        interactor.TryGetComponent(out Rigidbody rb);
        
        rb?.AddForce(transform.up * power, ForceMode.Acceleration);
    }
}
