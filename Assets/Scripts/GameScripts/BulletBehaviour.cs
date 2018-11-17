using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletBehaviour : MonoBehaviour {

    public float lifetime;
    public Vector3 direction;
    public float velocity;

    private Rigidbody2D _body;
    private float _startTime;

    void Start()
    {    
        _body = GetComponent<Rigidbody2D>();
        _body.velocity = direction * velocity;
        _startTime = Time.time;
    }

    void Update () {
		if ( Time.time - _startTime > lifetime)
        {
            Destroy(gameObject);
        }
	}
}
