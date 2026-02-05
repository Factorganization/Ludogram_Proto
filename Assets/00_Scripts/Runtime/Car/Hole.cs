using UnityEngine;

public class Hole : MonoBehaviour, IInteractable
{
    public void Interact(PlayerController interactor)
    {
        Destroy(gameObject);
    }
}
