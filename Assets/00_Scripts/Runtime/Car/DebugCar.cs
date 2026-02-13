using System;
using System.Threading.Tasks;
using CarScripts;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class DebugCar : MonoBehaviour
{
    [SerializeField] private CarController carController;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            ResetPosition().Forget();
        }
    }

    private async UniTask ResetPosition()
    {
        carController.GetRB().isKinematic = true;
        carController.enabled = false;
        await Task.Delay(TimeSpan.FromSeconds(0.25f));
        carController.transform.position += Vector3.up * 3;
        await Task.Delay(TimeSpan.FromSeconds(0.25f));
        carController.GetRB().isKinematic = false;
        carController.enabled = true;
    }
}
