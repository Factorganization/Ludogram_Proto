using UnityEngine;

public class RushState : EnemyState
{
    public RushState(EnemyBehavior _brain) : base(_brain) {}
    
    public override void Enter()
    {
        if (!brain.HoleGenerator)
            brain.ResetState();

        float distance = Vector3.Distance(brain.transform.position, brain.PlayerVehicule.transform.position);
        float t = (distance - brain.BumpOffset) / distance;
        if (t < 0) {t = 0;}
        
        Vector3 newTarget = Vector3.Lerp(brain.transform.position,brain.PlayerVehicule.transform.position,t);
        brain.SetNewTarget(newTarget, brain.PlayerVehicule.transform);
    }

    public override void Update()
    {
        brain.TryBump();
    }

    public override void Exit()
    {
        Reset();
    }

    void Reset()
    {
        
    }

}
