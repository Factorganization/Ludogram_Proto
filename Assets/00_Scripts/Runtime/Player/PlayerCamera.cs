using MortierFu.Shared;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCamera : MonoBehaviour
{
    [SerializeField] private Camera playerCamera;
    [SerializeField] private CinemachineCamera cinemachineCamera;
    [SerializeField] private Transform cameraRoot;
    
    // Public Getters
    public Camera PlayerMainCamera => playerCamera;
    public CinemachineCamera CinemachineCamera => cinemachineCamera;
    public Transform CameraRoot => cameraRoot;

    private void Awake()
    {
        if (playerCamera == null) Logs.LogError("[Player]: Player Main Camera is not assigned!", this);
        if (cinemachineCamera == null) Logs.LogError("[Player]: Cinemachine Brain is not assigned!", this);
        if (cameraRoot == null) Logs.LogError("[Player]: Camera Root is not assigned!", this);

    }
}
