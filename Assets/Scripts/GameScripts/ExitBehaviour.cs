/*
 * TDS (c) 2018 by Another Pointless Pun
 * TDS is licensed under a Creative Commons Attribution-ShareAlike 4.0 International License.
 * You should have received a copy of the license along with this work.  If not, see <http://creativecommons.org/licenses/by-sa/4.0/>.
 */
namespace Tds.GameScripts
{
    using UnityEngine;

    /// <summary>
    /// Behaviour defining what happens if a player collides with a level exit
    /// </summary>
    public class ExitBehaviour : MonoBehaviour
    {
        /// <summary>
        /// Tag of the object in the level which controls the in game state
        /// </summary>
        public string _inGameScriptTag;

        /// <summary>
        /// Resolved object containing the in state game behaviour
        /// </summary>
        private GameObject _inGameScriptObject;

        void Start()
        {
            _inGameScriptObject = GameObject.FindGameObjectWithTag(_inGameScriptTag);

            Contract.Requires(_inGameScriptObject != null, "No in game state beahaviour defined in ExitBehaviour." );
        }


        void OnCollisionEnter2D(Collision2D collision)
        {         
            // if the player collides with the exit, it will trigger a message to the in game state behaviour
            if (collision.gameObject.tag == GameTags.Player)
            {
                _inGameScriptObject.SendMessage(MessageNames.OnPlayerReachesExit, SendMessageOptions.RequireReceiver);
            }
        }
    }
}