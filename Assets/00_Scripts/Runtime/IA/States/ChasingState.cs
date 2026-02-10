using UnityEngine;

public class ChasingState : EnemyState
{
    public ChasingState(EnemyBehavior _brain) : base(_brain){}

    private float incrementSpeedTimer = 0;

    public override void Enter()
    {
        Reset();
        int randomDirection = Mathf.RoundToInt(Random.value*(3));
        Vector3 targetPos = brain.transform.position;

        switch (randomDirection)
        {
            case 0:
                targetPos = brain.PlayerVehicule.transform.position + (-brain.PlayerVehicule.transform.right)*brain.DistanceToVan -brain.PlayerVehicule.transform.forward*10;
                break;
            case 1:
                targetPos = brain.PlayerVehicule.transform.position + brain.PlayerVehicule.transform.forward*brain.DistanceToVan*0.75f;
                break;
            case 2:
                targetPos = brain.PlayerVehicule.transform.position + brain.PlayerVehicule.transform.right*brain.DistanceToVan -brain.PlayerVehicule.transform.forward*10;
                break;
            case 3:
                targetPos = brain.PlayerVehicule.transform.position + (-brain.PlayerVehicule.transform.forward)*brain.DistanceToVan*2f;
                break;
        }
        
        SetNewMoveTarget(targetPos);
    }

    public override void Update()
    {
        CheckIncrementingSpeed();
        CheckObstacleClose();
        brain.TryAttack();
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
        targetPos += Random.insideUnitSphere*brain.RandomOffsetRange;
        
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
                        brain.currentSpeed-= brain.SlowSpeedAmount;
                        if (brain.currentSpeed < 0)
                        {
                            brain.currentSpeed = 0;
                        }
                    }
                }
            }
        }
    }

    public override void Exit()
    {
        Reset();
    }

    void Reset()
    {
        incrementSpeedTimer = 0;
        brain.currentSpeed = brain.InitialSpeed;
    }
}
