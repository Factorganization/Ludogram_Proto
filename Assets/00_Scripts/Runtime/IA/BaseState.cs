using UnityEngine;

public class BaseState : EnemyState
{
    public BaseState(EnemyBehavior _brain) : base(_brain){}

    public override void Enter()
    {
    }

    public override void Update()
    {
        throw new System.NotImplementedException();
    }

    public override void Exit()
    {
        throw new System.NotImplementedException();
    }
}
