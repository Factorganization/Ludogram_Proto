using System;
using TMPro;
using UnityEngine;

public class Bank : MonoBehaviour, IInteractable
{
    public bool delivered;
    [SerializeField] private TextMeshPro text;
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
            if (text != null)
            {
                text.text = "";
            }
        }
        else
        {
            Debug.LogError("There is no GameManager");
        }
    }
}
