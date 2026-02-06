using System;
using UnityEngine;

public class EnemyBehavior : MonoBehaviour
{
    public HoleGenerator HoleGenerator => holeGenerator;
    public float ShootCooldown => shootCooldown;
    
    //State Date
    EnemyState _currentState;
    
    [SerializeField] private HoleGenerator holeGenerator;
    [SerializeField] private float shootCooldown = 2; //in seconds

    [Header("Test")] 
    [SerializeField] private EnemyStates testState;
    

    private void Start()
    {
        SetState(testState);
    }
    
    void SetState(EnemyStates state)
    {
        _currentState?.Exit();
        switch (state)
        {
            case(EnemyStates.BaseState):
                _currentState = new BaseState(this);
                break;
            
            case(EnemyStates.AttackState):
                _currentState = new AttackState(this);
                break;
        }
        _currentState?.Enter();
    }

    private void Update()
    {
        _currentState?.Update();
    }
}

enum EnemyStates
{
    BaseState,
    AttackState
}
