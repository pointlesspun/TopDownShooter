using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterBehaviour : MonoBehaviour
{
    public float _maxSpeed = 2.0f;

    private Rigidbody2D _body;
    private WeaponBase _weapon;
    private GameObject _player;

    private AttackParameters _attackDescription = new AttackParameters();

    void Start()
    {
        _player = GameObject.FindGameObjectWithTag("Player");
        _body = GetComponent<Rigidbody2D>();

        _weapon = GetComponent<WeaponBase>();
    }

    void Update()
    {
        _body.velocity = Vector3.zero;

        if (_player != null && _player.activeInHierarchy)
        {
            if (_weapon && _weapon.IsCooldownOver())
            {
                if (_weapon.IsInRange(_player))
                {
                    Vector3 attackDirection = (_player.transform.position - transform.position);
                    attackDirection.z = 0;
                    _attackDescription.target = _player;
                    _attackDescription.direction = attackDirection.normalized;
                    _weapon.Attack(_attackDescription);
                }
                else
                {
                    _body.velocity = (_player.transform.position - transform.position).normalized * _maxSpeed;
                }
            }
        }
    }
}
