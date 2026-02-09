using System.Collections.Generic;
using UnityEngine;

public class AttackState : EnemyState
{
    public AttackState(EnemyBehavior _brain) : base(_brain){}
    
    private List<Collider> _targetWalls = new List<Collider>();

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
        //_targetWalls = brain.HoleGenerator.targetWallstest;
        
        if (brain.PlayerVehicule && brain.PlayerVehicule.BankColliders.Length > 0)
        {
            Collider closestWall = null;
            foreach (var wall in brain.PlayerVehicule.BankColliders)
            {
                if (closestWall == null)
                {
                    closestWall = wall;
                    continue;
                }

                //Peut etre amélioré en recuperant le closestpoint mais flemme
                if (Vector3.Distance(brain.transform.position, closestWall.transform.position) >
                    Vector3.Distance(brain.transform.position, wall.transform.position))
                {
                    closestWall = wall;
                }
            }
            _targetWalls.Clear();
            _targetWalls.Add(closestWall);
        }

        
    }
}
