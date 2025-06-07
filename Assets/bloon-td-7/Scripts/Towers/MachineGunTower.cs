using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MachineGunTower : Tower
{
    [SerializeField] protected float projectileSpeed = 240f;
    protected float timeBetweenShots = 0.1f;
    protected float attackCooldownTimer = 0;
    protected int numExtraShotsInBurst = 2;
    protected int numShots = 2;
    protected int projectilePierce = 1;
    protected GameObject savedTarget = null;
    protected override void Start()
    {
        base.Start();
        UpgradeCosts = new int[] { 850, 950, 1050, 1200 };
        cost = 750;
        cooldown = 0f;
    }
    protected override void Attack(GameObject target)
    {
        if (numShots == numExtraShotsInBurst)
        {
            numShots = 0;
            savedTarget = target;
        }
        if (target == null) return;
        Projectile projectile = Instantiate(BTD7.GameManager.instance.projectile, transform.position, transform.rotation).GetComponent<Projectile>();
        projectile.Initialize(damage, projectileSpeed, projectilePierce, target.transform.position);
    }

    protected override void Update()
    {
        //base.Update();
        //savedTarget = GetTarget();
        attackCooldownTimer += Time.deltaTime;
        if (attackCooldownTimer >= timeBetweenShots)
        {
            attackCooldownTimer = 0;
            //Attack(savedTarget);
            base.Update();
            //numShots++;
        }
        return;
    }

    protected override void Upgrade(int level)
    {
        switch (level)
        {
            case 0:
                return;
            case 1:
                projectilePierce = 2;
                //damage++;
                projectileSpeed = 300f;
                return;
            case 2:
                // projectilePierce += 2;
                // cooldown *= 0.75f;
                // timeBetweenShots = 0.2f;
                // numExtraShotsInBurst++;
                // numShots = numExtraShotsInBurst;
                projectilePierce = 2;
                damage = 2;
                projectileSpeed = 400f;
                return;
            case 3:
                // damage += 3;
                // cooldown *= 0.75f;
                // timeBetweenShots = 0.15f;
                // numExtraShotsInBurst++;
                // numShots = numExtraShotsInBurst;
                projectilePierce = 2;
                damage = 2;
                timeBetweenShots = 0.05f;
                projectileSpeed = 420f;
                return;
            case 4:
                // cooldown *= 0.5f;
                // //damage += 7;
                // damage += 3;
                // projectilePierce += 5;
                // timeBetweenShots = 0.1f;
                // projectileSpeed = 500f;
                // numExtraShotsInBurst++;
                // numShots = numExtraShotsInBurst;
                projectilePierce = 10;
                damage = 3;
                timeBetweenShots = 0.025f;
                projectileSpeed = 450f;
                cooldown = 0.6f;
                return;
            default:
                projectilePierce = 10;
                damage = 2;
                numExtraShotsInBurst = 5;
                numShots = numExtraShotsInBurst;
                projectileSpeed = 500f;
                timeBetweenShots = 0.025f;
                cooldown = 0.5f;
                return;
        }
    }
}
