using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public abstract class WeaponBase : MonoBehaviour
{
    public float _range = -1.0f;
    public float _cooldown = 1.0f;
    public int _damage = 1;

    private float _lastAttackTime = -1.0f;

    public virtual bool IsCooldownOver()
    {
        return _lastAttackTime < 0 || Time.time - _lastAttackTime > _cooldown;
    } 

    public virtual bool IsInRange(GameObject target)
    {
        return (target.transform.position - transform.position).sqrMagnitude < _range * _range;
    }

    public void Attack(AttackParameters attackDescription)
    {
        // can we attack again  ?
        if (IsCooldownOver())
        {
            if (ExecuteAttack(attackDescription))
            {
                _lastAttackTime = Time.time;
            }
        }
    }

    protected abstract bool ExecuteAttack(AttackParameters attackDescription);
}

