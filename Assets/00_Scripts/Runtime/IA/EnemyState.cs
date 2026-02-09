using UnityEngine;

public abstract class EnemyState
{
    protected EnemyBehavior brain;

    protected EnemyState(EnemyBehavior _brain)
    {
        brain = _brain;
    }

    public abstract void Enter();
    public abstract void Update();
    public abstract void Exit();
}
