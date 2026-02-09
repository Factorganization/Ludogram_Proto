using UnityEngine;

public class Ladder : MonoBehaviour, IInteractable
{
    [SerializeField] private float power = 1000;
    
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
        interactor.transform.localPosition += new Vector3(0, 1 * power, -1f) ;
    }
}
