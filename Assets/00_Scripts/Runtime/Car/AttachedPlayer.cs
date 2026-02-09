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
        if (other.transform.GetComponentInParent<PlayerController>())
        {
            other.transform.parent = transform;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.transform.GetComponent<PlayerController>())
        {
            other.transform.parent = null;
        }
    }
    
}
