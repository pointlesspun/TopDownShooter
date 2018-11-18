using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterBehaviour : MonoBehaviour
{
    public float _maxSpeed = 2.0f;
    public bool _debugBehaviour = false;


    private string _state = "ok";
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
            if (_weapon == null)
            {
                _weapon = SelectWeapon();
            }

            if (_weapon && _weapon.IsCooldownOver())
            {
                AimWeapon(_weapon, _player);

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
                    var movementDirection = (_player.transform.position - transform.position);
                    movementDirection.z = 0;
                    movementDirection.Normalize();
                    _body.velocity = movementDirection * _maxSpeed;
                }
            }
            else if (_debugBehaviour)
            {
                _state = "No weapon.";
            }
        }
        else if (_debugBehaviour)
        {
            _state = "No player found.";
        }
    }

    private WeaponBase SelectWeapon()
    {
        WeaponBase result = null;

        for (int i = 0; i < transform.childCount; ++i)
        {
            var child = transform.GetChild(i).gameObject;

            if ( child.tag == "Weapon")
            {
                result = child.GetComponent<WeaponBase>();

                if (result != null)
                {
                    break;
                }
            }
        }

        return result;
    }

    private void AimWeapon(WeaponBase weapon, GameObject target)
    {
        var direction = (target.transform.position - transform.position);

        direction.z = 0;
        direction.Normalize();
        
        weapon.gameObject.transform.position = transform.position + direction * weapon._offsetFromOwner;
    }
}
