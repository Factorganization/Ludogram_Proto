using UnityEngine;

[CreateAssetMenu(fileName = "SO_PlayerStats", menuName = "ScriptableObjects/PlayerStats", order = 1)]
public class SO_PlayerStats : ScriptableObject
{
    public float Speed = 5f;
    [Tooltip("Jump height in meters.")]
    public float JumpHeight = 1.5f;
    public float LookSensitivity = 3f;
    [Range(-90f, 0f)]
    public float Gravity = -9.81f;
    [Tooltip("Vertical look range in degrees (up/down).")]
    public float UpDownLookRange = 80f;
    public float AirControl = 0.25f;
    
    [Header("Interaction")]
    [Tooltip("Radius of the interaction sphere cast in front of the player.")]
    public float InteractRadius = 0.25f;
    
    [Tooltip("Maximum distance for interaction checks in front of the player.")]
    public float InteractDistance = 10f;

    [Header("Rope")][Tooltip("la tension en Newton avant que le perso soit callback (c'est des Newton approximatif c pas une science exact)")] 
    public float MaxRopeTension = 150f;

    public float MaxRopeDistance = 10f;

}