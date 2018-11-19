/*
 * TDS (c) 2018 by Another Pointless Pun
 * TDS is licensed under a Creative Commons Attribution-ShareAlike 4.0 International License.
 * You should have received a copy of the license along with this work.  If not, see <http://creativecommons.org/licenses/by-sa/4.0/>.
 */
namespace Tds.GameStateScripts
{
    using Tds.GameScripts;
    using UnityEngine;
    using UnityEngine.SceneManagement;

    /// <summary>
    /// Expressed the behavior of the state when in game. Will track the state of the player.
    /// When the player is dead, will end the game state and go to the the title screen.
    /// </summary>
    public class InGameStateBehaviour : MonoBehaviour
    {
        public string _titleScreenScene;
        private GameObject _player;

        public void Start()
        {
            _player = GameObject.FindGameObjectWithTag(GameTags.Player);
        }

        public void Update()
        {
            if (_player == null || !_player.activeInHierarchy)
            {
                SceneManager.LoadScene(_titleScreenScene);
            }
        }
    }
}
