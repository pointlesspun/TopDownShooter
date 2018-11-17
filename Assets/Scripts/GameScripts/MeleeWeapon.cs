using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class MeleeWeapon : WeaponBase
{
    protected override bool ExecuteAttack(AttackParameters attackDescription)
    {
        // is the target in range ?
        if (IsInRange(attackDescription.target))
        {
            attackDescription.target.SendMessage("OnDamage", _damage);
            return true;
        }

        return false;
    }
}

