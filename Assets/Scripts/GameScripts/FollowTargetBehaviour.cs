/*
 * TDS (c) 2018 by Another Pointless Pun
 * TDS is licensed under a Creative Commons Attribution-ShareAlike 4.0 International License.
 * You should have received a copy of the license along with this work.  If not, see <http://creativecommons.org/licenses/by-sa/4.0/>.
 */
namespace Tds.GameScripts
{
    using UnityEngine;

    /// <summary>
    /// Behaviour following a given target's position
    /// </summary>
    public class FollowTargetBehaviour : MonoBehaviour
    {
        /// <summary>
        /// If the distance to the target exceeds this value, the behaviour will start following the target
        /// </summary>
        public float _deadZone = 1.0f;

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
                var positionDifference = transform.position - _target.transform.position;

                positionDifference.z = 0;

                if (_deadZone <= 0 || positionDifference.sqrMagnitude > _deadZone * _deadZone)
                {
                    // change the new position to make sure the target is in the dead zone
                    var newPosition = _target.transform.position + positionDifference.normalized * Mathf.Max(0, _deadZone);
                    newPosition.z = transform.position.z;
                    transform.position = newPosition;
                }
            }
        }
    }
}
