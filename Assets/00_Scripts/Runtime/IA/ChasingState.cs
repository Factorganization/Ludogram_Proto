using UnityEngine;

public class ChasingState : EnemyState
{
    public ChasingState(EnemyBehavior _brain) : base(_brain){}

    private float incrementSpeedTimer = 0;

    public override void Enter()
    {
        int randomDirection = Mathf.RoundToInt(Random.value*2);
        SetNewMoveTarget(brain.VanPositions[randomDirection].position);
    }

    public override void Update()
    {
        CheckIncrementingSpeed();
        CheckObstacleClose();
    }

    void CheckIncrementingSpeed()
    {
        if (incrementSpeedTimer < brain.IncrementSpeedTreshold)
        {
            incrementSpeedTimer += Time.deltaTime;
        }
        else
        {
            incrementSpeedTimer = 0;
            IncrementSpeed();
        }
    }

    private void IncrementSpeed()
    {
        brain.currentSpeed += brain.IncrementSpeedAmount;
    }

    private void SetNewMoveTarget(Vector3 targetPos)
    {
        // _brain.Movement.SetNewTarget()   a faire quand yaura un scriptmouvement ennemi
        targetPos += Random.insideUnitSphere*brain.RandomOffsetRange;
        //brain.transform.position = new Vector3(targetPos.x,brain.transform.position.y,targetPos.z);
        
        brain.SetNewTarget(targetPos, brain.PlayerVehicule.transform);
        
        CheckObstacleClose();
    }

    void CheckObstacleClose() //Refacto adapter à toute forme d'obastacle
    {
        CheckAIObstacle();
    }

    private void CheckAIObstacle()
    {
        Collider[] aiNear = Physics.OverlapSphere(brain.transform.position, brain.ObstacleDetectionRange, LayerMask.GetMask("AI"));
        if(aiNear.Length>0)
        {
           
            foreach (Collider col in aiNear)
            {
                EnemyBehavior ai = col.transform.GetComponentInParent<EnemyBehavior>();
                if (ai != brain)
                {
                    float selfDistance = Vector3.Distance(brain.transform.position, brain.PlayerVehicule.transform.position);
                    float aiDistance = Vector3.Distance(ai.transform.position, brain.PlayerVehicule.transform.position);

                    if (selfDistance > aiDistance)
                    {
                        //brain.movement.ReduceSpeed  
                        
                        //Refacto : c'est pour tester sans brain.Movement
                        brain.SetNewTarget(brain.transform.position -= (brain.transform.forward*3), brain.PlayerVehicule.transform);
                    }
                }
            }
        }
    }

    public override void Exit()
    {
    }
}
