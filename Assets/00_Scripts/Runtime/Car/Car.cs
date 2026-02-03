using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Car : MonoBehaviour
{
    public Rigidbody rigid;
    public WheelCollider wheel1, wheel2, wheel3, wheel4;
    public float drivespeed, steerspeed, maxSpeed;
    float horizontalInput, verticalInput;
    
    private bool breaking;

    private float currentSpeed;

    void Update()
    {
        horizontalInput = Input.GetAxis("Horizontal");
        verticalInput = Input.GetAxis("Vertical");

        currentSpeed = currentSpeed = Vector3.Dot(rigid.gameObject.transform.forward, rigid.linearVelocity);
        Debug.Log(currentSpeed);
        
    }

    void FixedUpdate()
    {
        Acceleration();
        Turn();
        ClampVelocity();
        
        Brake();
    }

    private float GetMotor()
    {
        return verticalInput * drivespeed;
    }

    private bool GetBreak()
    {
        return (Input.GetKey(KeyCode.Space));
    }

    private void Acceleration()
    {
        if (GetBreak()) return;
        
        wheel1.motorTorque = GetMotor();
        wheel2.motorTorque = GetMotor();
        wheel3.motorTorque = GetMotor();
        wheel4.motorTorque = GetMotor();
    }

    private void Turn()
    {
        wheel1.steerAngle = steerspeed * horizontalInput;
        wheel2.steerAngle = steerspeed * horizontalInput;
    }

    private void Brake()
    {
        if (GetBreak())
        {
            wheel1.motorTorque = 0;
            wheel2.motorTorque = 0;
            wheel3.motorTorque = 0;
            wheel4.motorTorque = 0;
            
            wheel1.brakeTorque = verticalInput * 5000000;
            wheel2.brakeTorque = verticalInput * 5000000;
            wheel3.brakeTorque = verticalInput * 5000000;
            wheel4.brakeTorque = verticalInput * 5000000;
        }
        else
        {
            wheel1.brakeTorque = 0;
            wheel2.brakeTorque = 0;
            wheel3.brakeTorque = 0;
            wheel4.brakeTorque = 0;
        }
    }

    private void ClampVelocity()
    {
        rigid.linearVelocity = new Vector3(Mathf.Clamp(rigid.linearVelocity.x, -maxSpeed, maxSpeed), rigid.linearVelocity.y, Mathf.Clamp(rigid.linearVelocity.z, -maxSpeed, maxSpeed));
    }
}