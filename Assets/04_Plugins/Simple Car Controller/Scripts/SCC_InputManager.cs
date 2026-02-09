//----------------------------------------------
//            Simple Car Controller
//
// Copyright © 2014 - 2023 BoneCracker Games
// http://www.bonecrackergames.com
//
//----------------------------------------------

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Input receiver through the Unity's new Input System.
/// </summary>
public class SCC_InputManager : SCC_Singleton<SCC_InputManager> {

    public DrivingInputs drivingInputs;       //  Actual inputs.
    private static SCC_InputActions inputActions;

    private void Awake() {
        

        //  Creating inputs.
        drivingInputs = new DrivingInputs();

    }

    private void Update() {

        //  Creating inputs.
        if (drivingInputs == null)
            drivingInputs = new DrivingInputs();

        //  Receive inputs from the controller.
        GetInputs();

    }

    /// <summary>
    /// Gets all inputs and registers button events.
    /// </summary>
    /// <returns></returns>
    public void GetInputs() {

        if (inputActions == null) {

            inputActions = new SCC_InputActions();
            inputActions.Enable();

        }

        drivingInputs.throttleInput = inputActions.Vehicle.Throttle.ReadValue<float>();
        drivingInputs.brakeInput = inputActions.Vehicle.Brake.ReadValue<float>();
        drivingInputs.steerInput = inputActions.Vehicle.Steering.ReadValue<float>();
        drivingInputs.handbrakeInput = inputActions.Vehicle.Handbrake.ReadValue<float>();

    }

}
