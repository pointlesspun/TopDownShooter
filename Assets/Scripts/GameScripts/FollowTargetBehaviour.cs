/*
 * TDS (c) 2018 by Another Pointless Pun
 * TDS is licensed under a Creative Commons Attribution-ShareAlike 4.0 International License.
 * You should have received a copy of the license along with this work.  If not, see <http://creativecommons.org/licenses/by-sa/4.0/>.
 */
namespace Tds.GameScripts
{
    using UnityEngine;

    public class FollowTargetBehaviour : MonoBehaviour
    {
        /// <summary>
        /// Object to follow
        /// </summary>
        private GameObject _target;

        public void Start()
        {
            _target = GameObject.FindGameObjectWithTag(GameTags.Player);
        }

        public void Update()
        {
            // match the target's position as long as the target is alive and active
            if (_target != null && _target.activeInHierarchy)
            {
                var position = _target.transform.position;
                position.z = transform.position.z;
                transform.position = position;
            }
        }
    }
}
