/*
 * TDS (c) 2018 by Another Pointless Pun
 * TDS is licensed under a Creative Commons Attribution-ShareAlike 4.0 International License.
 * You should have received a copy of the license along with this work.  If not, see <http://creativecommons.org/licenses/by-sa/4.0/>.
 */
namespace Tds.GameStateScripts
{
    using UnityEngine;
    using Tds.GameScripts;
    using UnityEngine.SceneManagement;

    public class IntrogameState : MonoBehaviour
    {
        /// <summary>
        /// Name of the scene containing the next level
        /// </summary>
        public string _nextSceneName;

        private bool _IsFireButtonDown = false;

        public void Update()
        {
            // check if the user clicks the firebutton - if so go to the next scene
            if (_IsFireButtonDown)
            {
                if (!Input.GetButton(InputNames.Fire1))
                {
                    SceneManager.LoadScene(_nextSceneName, LoadSceneMode.Single);
                }
            }

            _IsFireButtonDown = Input.GetButton(InputNames.Fire1);
        }

        public void OnPathComplete()
        {
            SceneManager.LoadScene(_nextSceneName, LoadSceneMode.Single);
        }
    }
}