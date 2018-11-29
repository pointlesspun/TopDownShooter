/*
 * TDS (c) 2018 by Another Pointless Pun
 * TDS is licensed under a Creative Commons Attribution-ShareAlike 4.0 International License.
 * You should have received a copy of the license along with this work.  If not, see <http://creativecommons.org/licenses/by-sa/4.0/>.
 */
namespace Tds.GameScripts
{
    using UnityEngine;

    public class PathPointInfo
    {
        public int _index;
        public GameObject _path;
    }

    public class PuppetteerBehaviour : MonoBehaviour
    {
        public GameObject _player;

        public GameObject _path;
        public int _firstPointIndex = 0;

        public GameObject _pathListener;

        public float _velocity = 1;
        public float _pathProgressionPointDistance = 0.25f;

        

        private int _currentIndex;
        public GameObject _currentPoint;

        private PlayerController _playerController;

        void Start()
        {
            _playerController = _player.GetComponent<PlayerController>();
            _playerController._isPuppet = true;
            _currentIndex = _firstPointIndex;
            _currentPoint = _path.transform.GetChild(_firstPointIndex).gameObject;
            _playerController.SetVelocity((_currentPoint.transform.position - _player.transform.position).normalized * _velocity);
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
            var distanceToPoint = (_currentPoint.transform.position - _player.transform.position).magnitude;
            
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

                    _playerController.SetVelocity((_currentPoint.transform.position - _player.transform.position).normalized * _velocity);

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