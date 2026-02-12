using System;
using UnityEngine;

public class DebugGround : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        other.transform.position = new Vector3(other.transform.position.x, 100,other.transform.position.z);
    }
}
