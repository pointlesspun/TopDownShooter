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
        /// <summary>
        /// Range in Unity units at which the weapon can be fired. This is just an
        /// indication for the AI at the moment as the range will not be enforced
        /// on the actual bullet (xxx todo)
        /// </summary>
        public float _range = -1.0f;

        /// <summary>
        /// Cooldown in seconds, the weapon cannot be fired until the cooldown is over.
        /// </summary>
        public float _cooldown = 1.0f;

        /// <summary>
        /// Damage done by the weapon 
        /// (xxx todo: move to floats)
        /// </summary>
        public int _damage = 1;

        /// <summary>
        /// Distance from the owner at which the weapon will attack from
        /// </summary>
        public float _offsetFromOwner = 0.3f;

        /// <summary>
        /// Last time this weapon performed an attack
        /// </summary>
        private float _lastAttackTime = -1.0f;

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
    }
}
