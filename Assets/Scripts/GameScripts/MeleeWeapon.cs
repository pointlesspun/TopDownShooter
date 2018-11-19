/*
 * TDS (c) 2018 by Another Pointless Pun
 * TDS is licensed under a Creative Commons Attribution-ShareAlike 4.0 International License.
 * You should have received a copy of the license along with this work.  If not, see <http://creativecommons.org/licenses/by-sa/4.0/>.
 */
namespace Tds.GameScripts
{
    /// <summary>
    /// Weapon which will cause damage to the target directly iff the target is in range
    /// </summary>
    public class MeleeWeapon : WeaponBase
    {
        protected override bool ExecuteAttack(AttackParameters attackDescription)
        {
            // is the target in range ?
            if (IsInRange(attackDescription._target))
            {
                attackDescription._target.SendMessage(MessageNames.OnDamage, _damage);
                return true;
            }

            return false;
        }
    }
}
