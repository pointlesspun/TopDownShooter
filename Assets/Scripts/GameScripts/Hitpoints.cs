/*
 * TDS (c) 2018 by Another Pointless Pun
 * TDS is licensed under a Creative Commons Attribution-ShareAlike 4.0 International License.
 * You should have received a copy of the license along with this work.  If not, see <http://creativecommons.org/licenses/by-sa/4.0/>.
 */
namespace Tds.GameScripts
{
    using UnityEngine;

    /// <summary>
    /// Component capturing the behaviour of a gamepoint with hitpoints. 
    /// </summary>
    public class Hitpoints : MonoBehaviour
    {
        /// <summary>
        /// Max hitpoints, current hitpoints should never exceed this
        /// </summary>
        public int _maxHitpoints = 5;

        /// <summary>
        /// Flag when set to true will ignore all damage taken
        /// </summary>
        public bool _isInvulnerable = false;

        /// <summary>
        /// Current hitpoints, if this drops to 0 or below, the gameojbect associated with this 
        /// </summary>
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
}
