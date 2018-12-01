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
    /// Right now it doesn't do much, but eventually should be shared between entities which require 
    /// global access through out the progression of a game.
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

        /// <summary>
        /// Current scaling for the level, this is used to scale hitpoints / damage / speed for enemies 
        /// as levels increase
        /// </summary>
        public static float _levelScale = 0;

        /// <summary>
        /// Updates the current Level scaling following an x-y curve which fits (x,y) = 1,1 / 3,2 / 6,3 / 10, 4 and so on
        /// Some usefull links:
        /// https://mycurvefit.com/
        /// https://www.desmos.com/calculator
        /// </summary>
        public static void UpdateLevelScaling()
        {
            _levelScale = 1.1f * Mathf.Pow(_level, 0.55f);
        }

        /// <summary>
        /// Starts a new game, setting the player health to -1 which should cause the player 
        /// to start with its max health and the level is set to 0
        /// </summary>
        public static void StartGame()
        {
            _level = 0;
        }

        /// <summary>
        /// Progress to the next level, capturing the current health and increasing the level and current level scale.
        /// </summary>
        /// <param name="currentPlayerHealth"></param>
        public static void IncreaseLevel()
        {
            _level++;
            UpdateLevelScaling();
        }
    }
}