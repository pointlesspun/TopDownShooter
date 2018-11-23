/*
 * TDS (c) 2018 by Another Pointless Pun
 * TDS is licensed under a Creative Commons Attribution-ShareAlike 4.0 International License.
 * You should have received a copy of the license along with this work.  If not, see <http://creativecommons.org/licenses/by-sa/4.0/>.
 */
namespace Tds.GameScripts
{
    /// <summary>
    /// States of animations for a mobile character with a facing direction. Currently that is only
    /// the player.
    /// </summary>
    public static class AnimationState
    {
        /// <summary>
        /// Character is idle and facing left.
        /// </summary>
        public static int IdleFacingLeft = 0;

        /// <summary>
        /// Character is idle and facing right.
        /// </summary>
        public static int IdleFacingRight = 1;

        /// <summary>
        /// Character is performing a forward motion while facing left.
        /// </summary>
        public static int MovingForwardFacingLeft = 2;

        /// <summary>
        /// Character is performing a backward motion while facing left.
        /// </summary>
        public static int MovingBackwardFacingLeft = 3;

        /// <summary>
        /// Character is performing a forward motion while facing right.
        /// </summary>
        public static int MovingForwardFacingRight = 4;

        /// <summary>
        /// Character is performing a forward motion while facing left.
        /// </summary>
        public static int MovingBackwardFacingRight = 5;
    }
}
