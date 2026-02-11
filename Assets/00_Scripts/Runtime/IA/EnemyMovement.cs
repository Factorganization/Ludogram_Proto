using UnityEngine;
using UnityEngine.PlayerLoop;

public class EnemyMovement
{
    public EnemyMovement(EnemyBehavior _brain)
    {
        brain = _brain;
        target = new GameObject().transform;
        target.position = brain.transform.position;
        target.parent = brain.transform;
    }

    public Vector3 CurrentTarget => target != null? target.position: Vector3.zero;
    public bool TargetReached() => Vector3.Distance(brain.transform.position, target.position) <= 3;
    
    private EnemyBehavior brain;
    private Transform target;
    
    public void UpdateMovement() //Refacto : c'est pour tester
    {
        if (!target) { return;}
        brain.transform.position =  Vector3.MoveTowards(brain.transform.position, target.position, Time.deltaTime*brain.currentSpeed);

        if (TargetReached())
        {
            brain.transform.LookAt(brain.transform.forward);
        }
        else
        {
            brain.transform.LookAt(target);
        }
    }

    public void UpdateTarget(Vector3 newPos, Transform newParent = null)
    {
        target.position = new Vector3(newPos.x,brain.transform.position.y,newPos.z);
        target.parent = newParent;
    }

}
