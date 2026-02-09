using System;
using UnityEngine;

public class IgnoreCollision : MonoBehaviour
{
    [SerializeField] private Collider thisCollider;
    [SerializeField] private Collider[] ignoredColliders;

    private void Start()
    {
        foreach (var collider in ignoredColliders)
        {
            Physics.IgnoreCollision(thisCollider, collider, true);
        }
    }
}
