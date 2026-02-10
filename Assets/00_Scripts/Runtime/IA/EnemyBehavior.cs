using System;
using CarScripts;
using UnityEngine;

public class EnemyBehavior : MonoBehaviour
{
    
    public float SlowSpeedAmount => slowSpeedAmount;
    
    public float PlayerDetectionRange => playerDetectionRange;
    public float NoPlayerDetectedLength => noPlayerDetectedLength;
    public float RandomOffsetRange => randomOffsetRange;
    public float ObstacleDetectionRange => obstacleDetectionRange;
    public float IncrementSpeedTreshold => incrementSpeedTreshold;
    public float IncrementSpeedAmount => incrementSpeedAmount;
    public int BulletPerShot => bulletPerShot;
    public int TotalBulletAmount => totalBulletAmount;
    public float ReloadTime => reloadTime;
    public CarController PlayerVehicule=> playerVehicule; // a remplacer par le nouveau script car
    public HoleGenerator HoleGenerator => holeGenerator;
    

    public float DistanceToVan => distanceToVan;
    public EnemyType Type => type;
   
    public float ShootCooldown => shootCooldown;
    
    //State Date
    EnemyState _currentState;

    [Header("Global References")]
    [SerializeField] private CarController playerVehicule; // a remplacer par le nouveau script car
    
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
    [SerializeField] private float slowSpeedAmount = 10;
    [SerializeField] private float distanceToVan = 15;
    
    
    [Header("Bumper")]
    
    
    [Header("Shooter")]
    [SerializeField] private HoleGenerator holeGenerator;
    [SerializeField] private float shootCooldown = 2; //in seconds
    [SerializeField] private int bulletPerShot=1;
    [SerializeField] private int totalBulletAmount=3;
    [SerializeField] private float reloadTime = 10;
    
    EnemyState _neutralState,_chasingState,_aimingState;
    private EnemyMovement _movement;
    private EnemyDetection _detection;
    
    [Header("Debug")]
    [SerializeField] private bool showGizmos = true;

    

    private void Awake()
    {
        _neutralState = new NeutralState(this);
        _chasingState = new ChasingState(this);
        _aimingState = new AimingState(this);
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

    public void ResetState()
    {
        SetState(_neutralState);
    }

    public void TryAttack() //refacto: hardcodé mais aaaaaaaaaaaaaa
    {
        if (_movement.TargetReached())
        {
            if(type == EnemyType.Shooter)
            {
                SetState(_aimingState);
            }
            else
            {
                
            }
        }
    }
    
    private void OnDrawGizmos()
    {
        if (!showGizmos) return;
        
        Gizmos.color = Color.black;
        Gizmos.DrawWireSphere(transform.position, playerDetectionRange);
        
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(transform.position, obstacleDetectionRange);
    }
}

enum EnemyStates
{
    NeutralState,
    ChasingState,
    AimingState
}

public enum EnemyType
{
    Bumper,
    Shooter
}
