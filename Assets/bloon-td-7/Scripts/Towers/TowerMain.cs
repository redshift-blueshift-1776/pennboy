using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;

public class TowerMain : Tower
{
    [SerializeField] protected float projectileSpeed = 240f;
    protected int projectilePierce = 1;
    protected override void Start()
    {
        base.Start();
        UpgradeCosts = new int[]{ 150,350,550, 800};
        cost = 50;
    }
    protected override void Attack(GameObject target)
    {
        if (target == null) return;
        Projectile projectile = Instantiate(BTD7.GameManager.instance.projectile, transform.position, transform.rotation).GetComponent<Projectile>();
        projectile.Initialize(damage, projectileSpeed, projectilePierce, target.transform.position);
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
                //projectileSpeed = 275f;
                return;
            case 2:
                projectilePierce = 2;
                //damage++;
                //cooldown *= 0.75f;
                projectileSpeed = 500f;
                return;
            case 3:
                //damage += 3;
                projectilePierce = 2;
                damage = 2;
                //cooldown *= 0.5f;
                projectileSpeed = 500f;
                return;
            case 4:
                // cooldown *= 0.5f;
                // damage += 7;
                projectilePierce = 2;
                damage = 30;
                //cooldown = 5;
                //projectilePierce += 5;
                projectileSpeed = 500f;
                return;
            default:
                cooldown *= 0.25f;
                damage += 7;
                projectilePierce += 3;
                return;
        }
    }
}
