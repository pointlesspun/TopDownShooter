using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionDamage : MonoBehaviour {

    public int damage = 1;
    public int damageToSelf = 0;
    public string friendlyTag = "Player";

    public bool destroyOnCollision = false;

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag != friendlyTag)
        {
            collision.gameObject.SendMessage("OnDamage", damage, SendMessageOptions.RequireReceiver);

            if ( damageToSelf != 0 )
            {
                gameObject.SendMessage("OnDamage", damage, SendMessageOptions.RequireReceiver);
            }

        }

        if (destroyOnCollision)
        {
            Destroy(gameObject);
        }

    }
}
