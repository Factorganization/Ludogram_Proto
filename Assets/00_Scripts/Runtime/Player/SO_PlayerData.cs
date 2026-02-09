using UnityEngine;

[CreateAssetMenu(fileName = "SO_PlayerStats", menuName = "ScriptableObjects/PlayerStats", order = 1)]
public class SO_PlayerStats : ScriptableObject
{
    public float Speed = 5f;
    [Tooltip("Jump height in meters.")]
    public float JumpHeight = 1.5f;
    [HideInInspector] public float JumpForce = 10f; // Legacy - kept for migration
    public float LookSensitivity = 3f;
    [Range(-90f, 0f)]
    public float Gravity = -9.81f;
    [Tooltip("Vertical look range in degrees (up/down).")]
    public float UpDownLookRange = 80f;
}