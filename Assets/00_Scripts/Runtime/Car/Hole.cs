using UnityEngine;

public class Hole : MonoBehaviour, IInteractable
{
    public Transform GetTransform() => transform;

    public void Interact(IInteractable.InteractAction action, Transform interactor)
    {
        var playerCharacter = interactor.GetComponent<PlayerCharacter>();
        if (playerCharacter != null)
        {
            Interact(playerCharacter);
        }
    }

    public void Interact(PlayerCharacter interactor)
    {
        Destroy(gameObject);
    }
}
