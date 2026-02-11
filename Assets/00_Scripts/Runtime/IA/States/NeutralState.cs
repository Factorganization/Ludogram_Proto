using UnityEngine;

public class NeutralState : EnemyState
{
    public NeutralState(EnemyBehavior _brain) : base(_brain){}

    public override void Enter()
    {
    }

    public override void Update()
    {
       brain.PlayerDetection();
    }

    public override void Exit()
    {
    }
}
