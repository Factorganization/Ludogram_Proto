using UnityEngine;

public class AttackState : EnemyState
{
    public AttackState(EnemyBehavior _brain) : base(_brain)
    {
        switch (brain.Type)
        {
            case EnemyType.Shooter:
                preciseState = new AimingState(brain);
                break;
            case EnemyType.Bumper:
                preciseState = new RushState(brain);
                break;
        }
    }

    private EnemyState preciseState;
    

    public override void Enter()
    {
        preciseState?.Enter();
    }

    public override void Update()
    {
        preciseState?.Update();
    }

    public override void Exit()
    {
        preciseState?.Exit();
    }

}
