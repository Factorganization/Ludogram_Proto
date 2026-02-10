using UnityEngine;

public class EnemyDetection 
{
    public EnemyDetection(EnemyBehavior _brain)
    {
        brain = _brain;
    }
    
    private EnemyBehavior brain;
    private float playerFarTimer;

    public bool DetectPlayer() 
    {
        if(!brain.PlayerVehicule)
            return false;
        
        return Vector3.Distance(brain.transform.position, brain.PlayerVehicule.transform.position) <= brain.PlayerDetectionRange;
    }

    public bool GoBackToNeutral()
    {
        if (playerFarTimer < brain.NoPlayerDetectedLength)
        {
            playerFarTimer += Time.deltaTime;
            return false;
        }
        else
        {
            playerFarTimer = 0;
            return true;
        }
    }
}
