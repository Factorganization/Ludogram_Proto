using System;
using CarScripts;
using UnityEngine;

public class EnemyBehavior : MonoBehaviour
{
    
    public Transform[] VanPositions => vanPositions;
    
    public float PlayerDetectionRange => playerDetectionRange;
    public float NoPlayerDetectedLength => noPlayerDetectedLength;
    public float RandomOffsetRange => randomOffsetRange;
    public float ObstacleDetectionRange => obstacleDetectionRange;
    public float IncrementSpeedTreshold => incrementSpeedTreshold;
    public float IncrementSpeedAmount => incrementSpeedAmount;
    public CarController PlayerVehicule=> playerVehicule; // a remplacer par le nouveau script car
    public HoleGenerator HoleGenerator => holeGenerator;
   
    public float ShootCooldown => shootCooldown;
    
    //State Date
    EnemyState _currentState;

    [Header("Global References")]
    [SerializeField] private CarController playerVehicule; // a remplacer par le nouveau script car

    [Tooltip("Left, Back, Right, Front")]
    [SerializeField] private Transform[] vanPositions;
    [SerializeField] private float randomOffsetRange = 5;

    public float currentSpeed;
    
    [Header("Global Variables")]
    [SerializeField] private EnemyType type;
    [SerializeField] private float initialSpeed = 10;
    [SerializeField] private float incrementSpeedAmount = 5;
    [SerializeField] private float incrementSpeedTreshold = 10;
    [SerializeField] private float obstacleDetectionRange = 5;
    [SerializeField] private float playerDetectionRange = 25;
    [SerializeField] private float noPlayerDetectedLength = 10;
    
    
    [Header("Bumper")]
    
    
    [Header("Shooter")]
    [SerializeField] private HoleGenerator holeGenerator;
    [SerializeField] private float shootCooldown = 2; //in seconds
    
    EnemyState _neutralState,_chasingState;
    private EnemyMovement _movement;
    private EnemyDetection _detection;
    
    [Header("Debug")]
    [SerializeField] private bool showGizmos = true;

    

    private void Awake()
    {
        _neutralState = new NeutralState(this);
        _chasingState = new ChasingState(this);
        _movement = new EnemyMovement(this);
        _detection = new EnemyDetection(this);
    }


    private void Start()
    {
        currentSpeed = initialSpeed;
        
        SetState(_neutralState);
    }
    
    void SetState(EnemyState newState)
    {
        _currentState?.Exit();
        _currentState = newState;
        _currentState?.Enter();
        
        Debug.Log("Entered state : "+ _currentState);
    }

    public void SetNewTarget(Vector3 newPos, Transform newParent=null)
    {
        _movement.UpdateTarget(newPos, newParent);
    }

    private void Update()
    {
        _currentState?.Update();
        
        _movement.UpdateMovement();
        
        if (_currentState!= _neutralState )
        {
            if (_detection.GoBackToNeutral() && !_detection.DetectPlayer())
            {
                SetState(_neutralState);
            }
        }
    }

    public void PlayerDetection()
    {
        if (_detection.DetectPlayer())
        {
            SetState(_chasingState);
        }
    }

    public void ResetTarget()
    {
        _movement.UpdateTarget(transform.position,transform);
    }
    
    private void OnDrawGizmos()
    {
        if (!showGizmos) return;
        
        Gizmos.color = Color.black;
        foreach (Transform pos in vanPositions)
        {
            Gizmos.DrawWireSphere(pos.position , RandomOffsetRange);
        }
        Gizmos.DrawWireSphere(transform.position, playerDetectionRange);
        
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(transform.position, obstacleDetectionRange);
    }
}

enum EnemyStates
{
    NeutralState,
    ChasingState,
    AttackState
}

enum EnemyType
{
    Bumper,
    Shooter
}
