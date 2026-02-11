using UnityEngine;

public class AimingState : EnemyState
{
    public AimingState(EnemyBehavior _brain) : base(_brain){}

    private float shootDelayTimer;
    private int currentBulletAmount;
    private float reloadTimer;

    public override void Enter()
    {
        Reset();
        ResetBulletAmount();
    }

    private void ResetBulletAmount()
    {
        currentBulletAmount = brain.TotalBulletAmount;
    }

    public override void Update()
    {
        if (currentBulletAmount <= 0)
        {
            Reload();
        }
        else
        {
            TryShoot();
        }
    }

    public override void Exit()
    {
        Reset();
    }

    void TryShoot()
    {
        if (shootDelayTimer < brain.ShootCooldown)
        {
            shootDelayTimer+=Time.deltaTime;
        }
        else
        {
            Shoot();
            shootDelayTimer = 0;
        }
    }
    
    private void Reload()
    {
        if (reloadTimer < brain.ReloadTime)
        {
            reloadTimer+=Time.deltaTime;
        }
        else
        {
            ResetBulletAmount();
            reloadTimer = 0;
        }
    }

    private void Shoot()
    {
        if (!brain.HoleGenerator)
            brain.ResetState();
        
        brain.HoleGenerator.PlaceHoles(brain.PlayerVehicule.BankColliders,brain.HolePerAttack);
        currentBulletAmount-= brain.HolePerAttack;
    }

    void Reset()
    {
        shootDelayTimer = 0;
        currentBulletAmount = 0;
        reloadTimer = 0; 
    }
}
