//----------------------------------------------
//            Simple Car Controller
//
// Copyright © 2014 - 2023 BoneCracker Games
// http://www.bonecrackergames.com
//
//----------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Input processor of the vehicle.
/// </summary>
[AddComponentMenu("BoneCracker Games/Simple Car Controller/SCC Input Processor")]
public class SCC_InputProcessor : MonoBehaviour {

    public DrivingInputs drivingInputs = new DrivingInputs();        //  Target inputs.
    
    public bool smoothInputs = true;        //  Smoothly lerp the inputs?
    public float smoothingFactor = 5f;      //  Smoothing factor.
    
    public static SCC_InputProcessor Instance;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else if (Instance != this)
            Destroy(this);
    }


    /// <summary>
    /// Overrides inputs with given inputs. Be sure to disable the receiveInputsFromInputManager while overriding inputs. 
    /// </summary>
    /// <param name="newDrivingInputs"></param>
    public void OverrideInputs(DrivingInputs newDrivingInputs) {

        if (!smoothInputs) {

            drivingInputs = newDrivingInputs;

        } else {

            drivingInputs.throttleInput = Mathf.MoveTowards(drivingInputs.throttleInput, newDrivingInputs.throttleInput, Time.deltaTime * smoothingFactor);
            drivingInputs.steerInput = Mathf.MoveTowards(drivingInputs.steerInput, newDrivingInputs.steerInput, Time.deltaTime * smoothingFactor);
            drivingInputs.brakeInput = Mathf.MoveTowards(drivingInputs.brakeInput, newDrivingInputs.brakeInput, Time.deltaTime * smoothingFactor);
            drivingInputs.handbrakeInput = Mathf.MoveTowards(drivingInputs.handbrakeInput, newDrivingInputs.handbrakeInput, Time.deltaTime * smoothingFactor);

        }

    }

}
