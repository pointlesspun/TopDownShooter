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
    /// Game state setting up all the persistent objects (currently only the player)
    /// </summary>
    public class IntrogameState : MonoBehaviour
    {
        /// <summary>
        /// Name of the scene containing the next level
        /// </summary>
        public string _nextSceneName;
        
        public void OnFireButton()
        {
            SceneManager.LoadScene(_nextSceneName, LoadSceneMode.Single);
        }

        public void OnPathComplete()
        {
            SceneManager.LoadScene(_nextSceneName, LoadSceneMode.Single);
        }
    }
}