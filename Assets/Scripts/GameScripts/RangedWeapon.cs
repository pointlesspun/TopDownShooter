/*
 * TDS (c) 2018 by Another Pointless Pun
 * TDS is licensed under a Creative Commons Attribution-ShareAlike 4.0 International License.
 * You should have received a copy of the license along with this work.  If not, see <http://creativecommons.org/licenses/by-sa/4.0/>.
 */
namespace Tds.GameScripts
{
    using System;
    using UnityEngine;

    /// <summary>
    /// Implementation of a generic ranged weapon
    /// </summary>
    public class RangedWeapon : WeaponBase
    {
        /// <summary>
        /// Game object spawned when weapon is fired
        /// </summary>
        public GameObject _bulletPrefab;

        /// <summary>
        /// Speed with which the bullet will be ejected
        /// </summary>
        public float _bulletSpeed = 1.0f;

        /// <summary>
        /// Life time in seconds of the bullet
        /// </summary>
        public float _bulletLifeTime = 2.0f;

        /// <summary>
        /// Size of the bullet
        /// </summary>
        public float _bulletScale = 1.0f;

        /// <summary>
        /// Level scaling for the bullets. When set above 0 allows enemies to shoot faster bullets as levels progress.
        /// </summary>
        public float _speedScalingPerLevel = 0;

      
        /// <summary>
        /// Override from WeaponBase outlining what is meant to happen, in this case a bullet
        /// will be spawned in the direction indicated by the attackDescription.
        /// </summary>
        /// <param name="attackDescription"></param>
        /// <returns></returns>
        protected override bool ExecuteAttack(AttackParameters attackDescription)
        {
            var bullet = Instantiate<GameObject>(_bulletPrefab);
            
            bullet.transform.localScale *= _bulletScale;
            bullet.transform.position = transform.position;
            bullet.name = "bullet " + gameObject.name;

            var bulletSettings = bullet.GetComponent<BulletBehaviour>();
            var levelScale = GlobalGameState._levelScale;

            bulletSettings._friendlyTag = gameObject.tag;
            bulletSettings._velocity = _bulletSpeed + levelScale * _speedScalingPerLevel;
            bulletSettings._direction = attackDescription._direction;
            bulletSettings._lifetime = _bulletLifeTime;
            bulletSettings._damage = _damage;
            bulletSettings._maxRange = _range;

            return true;
        }
    }
}
