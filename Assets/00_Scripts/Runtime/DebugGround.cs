using System;
using UnityEngine;

public class DebugGround : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player"|| other.transform.root.name == "PF_Vehicle")
            other.transform.position = new Vector3(other.transform.position.x, 100,other.transform.position.z);

        if (other.GetComponent<EnemyBehavior>() != null)
        {
            other.GetComponentInChildren<EnemyHealth>().LaunchAutoDestruction();
        }
    }
}
