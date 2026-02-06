using System.Collections.Generic;
using UnityEngine;

public class AttackState : EnemyState
{
    public AttackState(EnemyBehavior _brain) : base(_brain){}
    
    private List<Collider> _targetWalls;

    private float shootingTimer; //refacto
    
    public override void Enter()
    {
        AutoFindTarget();
    }

    public override void Update()
    {
        if (shootingTimer < brain.ShootCooldown)
        {
            shootingTimer+=Time.deltaTime;
            
        }
        else
        {
            brain.HoleGenerator.PlaceHoles(_targetWalls, 1);
            shootingTimer = 0;
        }
    }

    public override void Exit()
    {
    }

    void AutoFindTarget()
    {
        //c'est du test, ce sera a remplacer
        _targetWalls = brain.HoleGenerator.targetWallstest;

        // find List<Collider> targetWalls 
    }
}
