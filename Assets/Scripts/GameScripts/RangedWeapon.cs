using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class RangedWeapon : WeaponBase
{
    public GameObject prefab;

    public float bulletSpeed = 1.0f;
    public float bulletLifeTime = 2.0f;
    public float bulletScale = 1.0f;

    protected override bool ExecuteAttack(AttackParameters attackDescription)
    {
        var bullet = GameObject.Instantiate<GameObject>(prefab);
        var bulletBehaviour = bullet.GetComponent<BulletBehaviour>();

        bullet.transform.localScale *= bulletScale;
        bullet.transform.position = transform.position;
        bullet.transform.parent = transform;

        bulletBehaviour.velocity = bulletSpeed;
        bulletBehaviour.direction = attackDescription.direction;
        bulletBehaviour.lifetime = bulletLifeTime;

        return true;
    }
}

