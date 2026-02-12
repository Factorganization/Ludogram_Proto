using System;
using UnityEngine;

public class Hole : MonoBehaviour, IInteractable
{
    public Transform GetTransform() => transform;

    [SerializeField] private float moneyLossPerSecond = 1f;
    private GameManager gm; 

    private void Start()
    {
        gm = GameManager.Instance;
        gm.holesCount++;
    }

    private void Update()
    {
        gm?.UpdateMoneyAmount(-moneyLossPerSecond*Time.deltaTime);
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
        gm.holesCount--;
        Destroy(gameObject);
    }
}
