/*
 * TDS (c) 2018 by Another Pointless Pun
 * TDS is licensed under a Creative Commons Attribution-ShareAlike 4.0 International License.
 * You should have received a copy of the license along with this work.  If not, see <http://creativecommons.org/licenses/by-sa/4.0/>.
 */
namespace Tds.GameScripts
{
    using UnityEngine;

    /// <summary>
    /// Base class for all weapons used
    /// </summary>
    public abstract class WeaponBase : MonoBehaviour
    {
        public int _priority = 0;

        /// <summary>
        /// Range in Unity units at which the weapon can be fired. This is just an
        /// indication for the AI at the moment as the range will not be enforced
        /// on the actual bullet 
        /// </summary>
        public float _range = -1.0f;

        /// <summary>
        /// Cooldown in seconds, the weapon cannot be fired until the cooldown is over.
        /// </summary>
        public float _cooldown = 1.0f;

        /// <summary>
        /// Damage done by the weapon 
        /// </summary>
        public float _damage = 1;

        /// <summary>
        /// Distance from the owner at which the weapon will attack from
        /// </summary>
        public float _offsetFromOwner = 0.3f;

        /// <summary>
        /// Last time this weapon performed an attack
        /// </summary>
        private float _lastAttackTime = -1.0f;

        /// <summary>
        /// Cooldown scaling for the bullets. When set above 0 decreases enemy cooldowns as levels progress.
        /// </summary>
        public float _cooldownLevelScaling = 0;

        /// <summary>
        /// Damage scaling for the bullets. When set above 0 increases enemy damage as levels progress.
        /// </summary>
        public float _damageScalingPerLevel = 0;

        /// <summary>
        /// Range scaling for the bullets. When set above 0 allows enemies to shoot bullets further as levels progress.
        /// </summary>
        public float _rangeScalingPerLevel = 0;

        /// <summary>
        /// Weapon cooldowns cannot drop below this value
        /// </summary>
        public float _miniumCooldown = 0.1f;

        public virtual void Start()
        {
            var levelScale = GlobalGameState._levelScale;

            _cooldown = Mathf.Max(_miniumCooldown, _cooldown - _cooldownLevelScaling * levelScale);
            _damage = _damage + levelScale * _damageScalingPerLevel;
            _range = _range + levelScale * _rangeScalingPerLevel;
        }

        public virtual bool IsCooldownOver()
        {
            return _lastAttackTime < 0 || Time.time - _lastAttackTime > _cooldown;
        }

        public virtual bool IsInRange(GameObject target)
        {
            var distance = (target.transform.position - transform.position).sqrMagnitude;
            return distance < _range * _range;
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

        // to be implemented by subclasses
        protected abstract bool ExecuteAttack(AttackParameters attackDescription);

        // what happens if a similar weapon has been picked up
        public abstract void Merge(WeaponBase other);
    }
}
