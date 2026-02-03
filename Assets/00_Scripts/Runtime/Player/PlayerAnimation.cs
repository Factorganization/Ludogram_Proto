using System;
using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private PlayerController pc;

    private float x, y, speed;
    private float targetSpeed;

    private void Update()
    {
        UpdateMovementAnimation();
    }

    private void UpdateMovementAnimation()
    {
        x = Mathf.Lerp(x, pc.MoveVector.x, Time.deltaTime * 15);
        y = Mathf.Lerp(y, pc.MoveVector.y, Time.deltaTime * 15);

        if (pc.CurrentSpeed == pc.sprintSpeed)
        {
            targetSpeed = 1;
            animator.SetFloat("speed", speed);
        }
        else
        {
            targetSpeed = 0;
            animator.SetFloat("speed", speed);
        }
        
        animator.SetFloat("x", x);
        animator.SetFloat("y", y);
    }
}
