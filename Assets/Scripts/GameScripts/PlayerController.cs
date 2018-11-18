using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

    private Rigidbody2D _body;
    

    public float velocityDampening = 8;
    public float maxVelocity = 4;
    public float minVelocity = 0.0005f;

    public float velocityIncrease = 32;

    private WeaponBase _weapon;
    private AttackParameters _attackDescription = new AttackParameters();
    private Camera _camera;

    void Start () {
        _body = GetComponent<Rigidbody2D>();

        _camera = Camera.main;
        _weapon = SelectWeapon();

    }
	
	void Update () {

        var velocity = _body.velocity;

        velocity *= 1.0f - (velocityDampening * Time.deltaTime);

        velocity += Vector2.up * Input.GetAxis("Vertical") * velocityIncrease * Time.deltaTime;
        velocity += Vector2.right * Input.GetAxis("Horizontal") * velocityIncrease * Time.deltaTime;

        var direction = velocity.normalized;
        var magnitude = velocity.magnitude;

        if ( magnitude > maxVelocity )
        {
            magnitude = maxVelocity;
        }
        else if ( magnitude < minVelocity )
        {
            magnitude = 0;
        }
        
        _body.velocity = magnitude * direction;

        var mouseScreenPosition = Input.mousePosition;
        var mouseWorldPosition = _camera.ScreenToWorldPoint(new Vector3(mouseScreenPosition.x, mouseScreenPosition.y, _camera.nearClipPlane));
        
        mouseWorldPosition.z = 0;

        var attackDirection = (mouseWorldPosition - transform.position).normalized;
        _weapon.transform.position = transform.position + attackDirection  * _weapon._offsetFromOwner;

        if (Input.GetButton("Fire1") && _weapon != null && _weapon.IsCooldownOver())
        {
        
            _attackDescription.direction = attackDirection;
            _weapon.Attack(_attackDescription);
        }
    }


    private WeaponBase SelectWeapon()
    {
        WeaponBase result = null;

        for (int i = 0; i < transform.childCount; ++i)
        {
            var child = transform.GetChild(i).gameObject;

            if (child.tag == "Weapon")
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
}
