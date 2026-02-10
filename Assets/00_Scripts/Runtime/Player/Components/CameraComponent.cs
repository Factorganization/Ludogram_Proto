using System;
using System.Collections;
using Unity.Cinemachine;
using UnityEngine;
using Cysharp.Threading.Tasks;
using UnityEditor.Experimental.GraphView;

public class CameraComponent : PlayerComponent
{

    private LayerMask _layerMaskToSet;
    
    public CameraComponent(PlayerCharacter character) : base(character)
    {
    }
    
    public override void Initialize()
    {
        // Set the player layer to the corresponding layer depending on his PlayerIndex
        _layerMaskToSet = LayerMask.NameToLayer($"Player_{character.PlayerIndex}");
        
        // Set this layer to the player gameobject and all its children
        SetLayerRecursively(character.gameObject, _layerMaskToSet);
        
        // Then exclude this layer from this player main camera culling mask 
        character.MainCamera.cullingMask = ~(1 << LayerMask.NameToLayer($"Player_{character.PlayerIndex}"));
    }

    private void SetLayerRecursively(GameObject characterGameObject, LayerMask layerMaskToSet)
    {
        // Si le jeu crash c'est ptet a cause de ça... a méditer
        characterGameObject.layer = layerMaskToSet;
        
        foreach (Transform child in characterGameObject.transform)
        {
            SetLayerRecursively(child.gameObject, layerMaskToSet);
        }
    }

    private CinemachineCamera _cinemachine;
    private CinemachineBasicMultiChannelPerlin _perlin;

    private void Awake()
    {
        _perlin = _cinemachine.GetComponent<CinemachineBasicMultiChannelPerlin>();
    }

    public enum ShakeType
    {
        SMALL,
        MEDIUM,
        BIG
    }
    
    public void CallCameraShake(Camera camera, ShakeType shakeType)
    {
        switch (shakeType)
        {
            case ShakeType.SMALL:
                ShakeCamera( 3, 0.13f);
                break;
            case ShakeType.MEDIUM:
                ShakeCamera( 5, 0.18f);
                break;
            case ShakeType.BIG:
                ShakeCamera( 10, 0.22f);
                break;
        }
    }
    
    private async UniTask ShakeCamera(float shakeIntensity, float shakeTime, float delay = 0)
    {
        if (delay > 0)
            await UniTask.Delay(TimeSpan.FromSeconds(delay), ignoreTimeScale: true);
        
        _perlin.AmplitudeGain = shakeIntensity;
        _perlin.FrequencyGain = 10;
        
        await UniTask.Delay(TimeSpan.FromSeconds(shakeTime), ignoreTimeScale: true);
        
        _perlin.AmplitudeGain = 0.5f;
        _perlin.FrequencyGain = 0.2f;
    }
}