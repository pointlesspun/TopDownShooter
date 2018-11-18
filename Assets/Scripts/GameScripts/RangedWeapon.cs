using UnityEngine;

public class RangedWeapon : WeaponBase
{
    public GameObject _bulletPrefab;

    public float bulletSpeed = 1.0f;
    public float bulletLifeTime = 2.0f;
    public float bulletScale = 1.0f;

    protected override bool ExecuteAttack(AttackParameters attackDescription)
    {
        var bullet = Instantiate<GameObject>(_bulletPrefab);
        var bulletBehaviour = bullet.GetComponent<BulletBehaviour>();

        bullet.transform.localScale *= bulletScale;
        bullet.transform.position = transform.position;
        bullet.name = "bullet " + gameObject.name;

        if (InGameStateBehaviour._inGameStateObject != null)
        {
            // assign bullet to the in game state, so when the game state is destroyed
            // all the bullets are removed as well.
            bullet.transform.parent = InGameStateBehaviour._inGameStateObject.transform.parent;
        }

        bulletBehaviour.velocity = bulletSpeed;
        bulletBehaviour.direction = attackDescription.direction;
        bulletBehaviour.lifetime = bulletLifeTime;

        return true;
    }

  

}

