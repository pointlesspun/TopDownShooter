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
    /// Expressed the behavior of the state when in the title screen. When the player presses the
    /// fire button, the game will start.
    /// </summary>
    public class TitleScreenStateBehaviour : MonoBehaviour
    {        
        /// <summary>
        /// Name of the next scene to spawn
        /// </summary>
        public string _nextSceneName = "";

        public void OnFireButton()
        {
            SceneManager.LoadScene(_nextSceneName, LoadSceneMode.Single);
        }
    }
}
