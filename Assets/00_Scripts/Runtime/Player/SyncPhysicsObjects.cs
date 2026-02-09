using System;
using UnityEngine;

public class SyncPhysicsObjects : MonoBehaviour
{
    private Rigidbody rb;
    private ConfigurableJoint joint;
    
    [SerializeField] Rigidbody animatedRb;

    [SerializeField] private bool syncAnimation = false;
    
    //Keep track for starting frame update
    private Quaternion startLocalRotation;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        joint = GetComponent<ConfigurableJoint>();
        
        //Store the starting local rotation
        startLocalRotation = transform.localRotation;
    }

    public void UpdateJointFromAnimation()
    {
        if (!syncAnimation) return;
        
        ConfigurableJointExtensions.SetTargetRotationLocal(joint, animatedRb.transform.localRotation, startLocalRotation);
    }
}
