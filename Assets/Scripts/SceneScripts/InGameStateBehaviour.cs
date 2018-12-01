/*
 * TDS (c) 2018 by Another Pointless Pun
 * TDS is licensed under a Creative Commons Attribution-ShareAlike 4.0 International License.
 * You should have received a copy of the license along with this work.  If not, see <http://creativecommons.org/licenses/by-sa/4.0/>.
 */
namespace Tds.SceneScripts
{
    using UnityEngine;
    using UnityEngine.SceneManagement;

    /// <summary>
    /// Expressed the behavior of the state when in game. Will track the state of the player.
    /// When the player is dead, will end the game state and go to the the title screen.
    /// </summary>
    public class InGameStateBehaviour : MonoBehaviour
    {
        /// <summary>
        /// Name of the scene containing the title page (for game over situations)
        /// </summary>
        public string _titleScreenScene;

        /// <summary>
        /// Name of the scene containing the next level
        /// </summary>
        public string _nextLevelScene;

        /// <summary>
        /// Player object- if this is null or not active, assumes it's a game over state and
        /// will load the title screen
        /// </summary>
        private GameObject _player;

        public void Start()
        {
            _player = GameObject.FindGameObjectWithTag(GameTags.Player);
        }

        public void Update()
        {
            if (_player == null || !_player.activeInHierarchy)
            {
                // game over...
                SceneManager.LoadScene(_titleScreenScene);
            }
        }

        /// <summary>
        /// Callback when the player reaches the exit so the game needs to transition to the next scene.
        /// </summary>
        public void OnPlayerReachesExit()
        {
            SceneManager.LoadScene(_nextLevelScene);
        }
    }
}
