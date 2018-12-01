/*
 * TDS (c) 2018 by Another Pointless Pun
 * TDS is licensed under a Creative Commons Attribution-ShareAlike 4.0 International License.
 * You should have received a copy of the license along with this work.  If not, see <http://creativecommons.org/licenses/by-sa/4.0/>.
 */
namespace Tds.GameScripts
{
    using UnityEngine;
    using UnityEngine.SceneManagement;

    /// <summary>
    /// Global state of the current game. Keeps references to most relevant (persistent) objects, basically a highly
    /// coupled IoC container.
    /// 
    /// Right now it doesn't do much, but eventually should be shared between entities which require 
    /// global access through out the progression of a game.
    /// </summary>
    public class GameStateBehaviour : MonoBehaviour
    {        
        /// <summary>
        /// Current level
        /// </summary>
        public int _level = 0;

        /// <summary>
        /// Current scaling for the level, this is used to scale hitpoints / damage / speed for enemies 
        /// as levels increase
        /// </summary>
        public float _levelScale = 0;

        /// <summary>
        /// Returns the game state's gamestate behaviour if any are defined
        /// </summary>
        /// <returns></returns>
        public static GameStateBehaviour Resolve()
        {
            var gameStateObject = GameObject.FindGameObjectWithTag(GameTags.GameState);
            return gameStateObject ? gameStateObject.GetComponent<GameStateBehaviour>() : null;
        }

        /// <summary>
        /// Starts a new game, setting the player health to -1 which should cause the player 
        /// to start with its max health and the level is set to 0
        /// </summary>
        public void Start()
        {
            // clean up any existing game states
            var otherGameStates = GameObject.FindGameObjectsWithTag(GameTags.GameState);

            if (otherGameStates.Length > 1)
            {
                for (int i = 0; i< otherGameStates.Length; ++i)
                {
                    if (otherGameStates[i] != gameObject)
                    {
                        Destroy(otherGameStates[i]);
                    }
                }
            }

            _level = 0;
            DontDestroyOnLoad(gameObject);
        }

        /// <summary>
        /// Callback when the player gets enabled, starts listening to sceneloaded events
        /// </summary>
        public void OnEnable()
        {
            SceneManager.sceneLoaded += this.OnLoadCallback;
        }

        /// <summary>
        /// Callback when the player gets disabled, stops listening to sceneloaded events
        /// </summary>
        public void OnDisable()
        {
            SceneManager.sceneLoaded -= this.OnLoadCallback;
        }

        public void OnLoadCallback(Scene scene, LoadSceneMode sceneMode)
        {
            _level++;
            _levelScale = 1.1f * Mathf.Pow(_level, 0.55f);
        }
    }
}