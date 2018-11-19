/*
 * TDS (c) 2018 by Another Pointless Pun
 * TDS is licensed under a Creative Commons Attribution-ShareAlike 4.0 International License.
 * You should have received a copy of the license along with this work.  If not, see <http://creativecommons.org/licenses/by-sa/4.0/>.
 */
namespace Tds.GameScripts
{
    using UnityEngine;

    /// <summary>
    /// Class which contains the parameters for executing an attack
    /// </summary>
    public class AttackParameters
    {
        /// <summary>
        /// Target of the attack
        /// </summary>
        public GameObject _target;

        /// <summary>
        /// Direction of the attack
        /// </summary>
        public Vector3 _direction;
    }
}
