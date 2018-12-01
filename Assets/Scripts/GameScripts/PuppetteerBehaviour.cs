/*
 * TDS (c) 2018 by Another Pointless Pun
 * TDS is licensed under a Creative Commons Attribution-ShareAlike 4.0 International License.
 * You should have received a copy of the license along with this work.  If not, see <http://creativecommons.org/licenses/by-sa/4.0/>.
 */
namespace Tds.GameScripts
{
    using UnityEngine;

    /// <summary>
    /// Contains information of a path
    /// </summary>
    public class PathPointInfo
    {
        /// <summary>
        /// Current object in the path to go to
        /// </summary>
        public int _index;

        /// <summary>
        /// Game object which children make up a path
        /// </summary>
        public GameObject _path;
    }

    /// <summary>
    /// Behaviour which can take control of an object, controlling their movements
    /// </summary>
    public class PuppetteerBehaviour : MonoBehaviour
    {
        /// <summary>
        /// Controlled game object (must be a player in this implementation)
        /// </summary>
        public GameObject _puppet;

        /// <summary>
        /// Path to follow
        /// </summary>
        public GameObject _path;

        /// <summary>
        /// Index of the child which is the first point int the path
        /// </summary>
        public int _firstPointIndex = 0;

        /// <summary>
        /// Game object listening for path objects
        /// </summary>
        public GameObject _pathListener;

        /// <summary>
        /// Velocity of the object following the path
        /// </summary>
        public float _velocity = 1;

        /// <summary>
        /// Distance to pathpoints which causes the next path to go to 
        /// </summary>
        public float _pathProgressionPointDistance = 0.25f;
        
        /// <summary>
        /// Current index in the path
        /// </summary>
        private int _currentIndex;

        /// <summary>
        /// Current path point to go to
        /// </summary>
        public GameObject _currentPoint;

        private PlayerController _playerController;

        void Start()
        {
            _playerController = _puppet.GetComponent<PlayerController>();
            _playerController._isPuppet = true;
            _currentIndex = _firstPointIndex;
            _currentPoint = _path.transform.GetChild(_firstPointIndex).gameObject;
            _playerController.SetVelocity((_currentPoint.transform.position - _puppet.transform.position).normalized * _velocity);
        }

        void Update()
        {
            if (_currentIndex < _path.transform.childCount)
            {
                UpdateVelocity();
            }
        }

        public void OnDestroy()
        {
            _playerController._isPuppet = false;
        }

        public void UpdateVelocity()
        {
            var distanceToPoint = (_currentPoint.transform.position - _puppet.transform.position).magnitude;
            
            if (distanceToPoint < _pathProgressionPointDistance )
            {
                _currentIndex++;
                if (_currentIndex < _path.transform.childCount)
                {
                    _currentPoint = _path.transform.GetChild(_currentIndex).gameObject;

                    if (_pathListener != null)
                    {
                        _pathListener.SendMessage(MessageNames.OnPointReached, new PathPointInfo()
                        {
                            _path = _path,
                            _index = _currentIndex
                        }, SendMessageOptions.DontRequireReceiver);
                    }

                    _playerController.SetVelocity((_currentPoint.transform.position - _puppet.transform.position).normalized * _velocity);

                }
                else
                {
                    if (_pathListener != null)
                    {
                        _pathListener.SendMessage(MessageNames.OnPathComplete);
                    }

                    _playerController.SetVelocity(Vector3.zero);
                }
            }

        }
    }
}