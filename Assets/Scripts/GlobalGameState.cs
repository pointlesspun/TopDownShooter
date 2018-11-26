/*
 * TDS (c) 2018 by Another Pointless Pun
 * TDS is licensed under a Creative Commons Attribution-ShareAlike 4.0 International License.
 * You should have received a copy of the license along with this work.  If not, see <http://creativecommons.org/licenses/by-sa/4.0/>.
 */
namespace Tds
{
    using UnityEngine;

    /// <summary>
    /// Global state of the current game. Keeps references to most relevant (persistent) objects, basically a highly
    /// coupled IoC container.
    /// 
    /// Right now it doesn't do much, but eventually should be shared between entities which require a
    /// global overview.
    /// </summary>
    public static class GlobalGameState 
    {
        /// <summary>
        /// Object holding the player
        /// </summary>
        public static GameObject _playerObject;

        /// <summary>
        /// Current level
        /// </summary>
        public static int _level = 0;
    }
}