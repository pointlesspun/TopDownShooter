/*
 * TDS (c) 2018 by Another Pointless Pun
 * TDS is licensed under a Creative Commons Attribution-ShareAlike 4.0 International License.
 * You should have received a copy of the license along with this work.  If not, see <http://creativecommons.org/licenses/by-sa/4.0/>.
 */
namespace Tds.GameScripts
{
    using UnityEngine;

    using Tds.DungeonGeneration;

    /// <summary>
    /// Class which connects a trigger to a node and when triggered, informs the Director.
    /// The director keeps tracks of the player's progress. Whenever the player moves
    /// into a room, the director will be informed and can take action.
    /// </summary>
    public class TraversalNodeTriggerBehaviour : MonoBehaviour
    {
        public Director _director;
        public TraversalNode _node;

        public void OnTriggerEnter2D(Collider2D collider)
        {
            _director.OnTrigger(collider, _node);
        }
    }
}
