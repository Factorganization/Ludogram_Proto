using UnityEngine;

public class Ladder : MonoBehaviour, IInteractable
{
    [SerializeField] private float power = 1000;
    public void Interact(PlayerController interactor)
    {
        interactor.transform.localPosition += new Vector3(0, 1 * power, -1f) ;
    }
}
