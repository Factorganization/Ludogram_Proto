using UnityEngine;

[CreateAssetMenu(fileName = "SO_PlayerStats", menuName = "ScriptableObjects/PlayerStats", order = 1)]
public class SO_PlayerStats : ScriptableObject
{
    public float Speed = 5f;
    public float JumpForce = 10f;
    public float LookSensitivity = 3f;
    public float Gravity = -9.81f;
}