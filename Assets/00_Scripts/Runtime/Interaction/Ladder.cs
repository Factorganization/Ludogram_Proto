using UnityEngine;

public class Ladder : MonoBehaviour, IInteractable
{
    [SerializeField] private float power = 50;
    public void Interact(PlayerController interactor)
    {
        interactor.TryGetComponent(out Rigidbody rb);
        
        rb?.AddForce(transform.up * power, ForceMode.Impulse);
    }
}
