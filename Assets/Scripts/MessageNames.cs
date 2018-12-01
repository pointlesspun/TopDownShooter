/*
 * TDS (c) 2018 by Another Pointless Pun
 * TDS is licensed under a Creative Commons Attribution-ShareAlike 4.0 International License.
 * You should have received a copy of the license along with this work.  If not, see <http://creativecommons.org/licenses/by-sa/4.0/>.
 */
namespace Tds
{
    /// <summary>
    /// Well known names of the message end points
    /// </summary>
    public static class MessageNames
    {
        public static string OnDamage = "OnDamage";
        public static string OnWeaponDestroyed = "OnWeaponDestroyed";
        public static string OnPlayerReachesExit = "OnPlayerReachesExit";

        public static string OnPointReached = "OnPointReached";
        public static string OnPathComplete = "OnPathComplete";

        public static string OnPickupItem = "OnPickupItem";
        public static string OnPickupHealth = "OnPickupHealth";

        public static string OnFireButton = "OnFireButton";
    }
}
