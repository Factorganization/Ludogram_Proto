using System;
using UnityEngine;

public class Bank : MonoBehaviour, IInteractable
{
    public bool delivered;
    public Transform GetTransform()
    {
        return transform;
    }

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
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnBankInteraction(this);
        }
        else
        {
            Debug.LogError("There is no GameManager");
        }
    }
}
