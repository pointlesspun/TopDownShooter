using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hitpoints : MonoBehaviour {

    public int _maxHitpoints = 5;
    public bool _isInvulnerable = false;
    public int _hitpoints = 5;
   

    void OnDamage(int damage)
    {
        if (!_isInvulnerable)
        {
            _hitpoints -= damage;

            if (_hitpoints <= 0)
            {
                Destroy(gameObject);
            }
        }
    }
}
