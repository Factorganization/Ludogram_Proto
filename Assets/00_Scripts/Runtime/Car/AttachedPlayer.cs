using System;
using UnityEngine;

public class AttachedPlayer : MonoBehaviour
{
    [SerializeField] private Transform carRef;

    private void Start()
    {
        transform.parent = null;
    }

    private void Update()
    {
        FollowCar();
    }

    private void FollowCar()
    {
        transform.position = carRef.position;
        transform.rotation = carRef.rotation;
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log($"{other.gameObject.name}");
        if (other.transform.TryGetComponent<PlayerCharacter>(out var player))
        {
            other.transform.parent = transform;
            player.Controller.EnterVehicle(transform);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.transform.TryGetComponent<PlayerCharacter>(out var player))
        {
            other.transform.parent = null;
            player.Controller.ExitVehicle();
        }
    }
    
}
