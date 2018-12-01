/*
 * TDS (c) 2018 by Another Pointless Pun
 * TDS is licensed under a Creative Commons Attribution-ShareAlike 4.0 International License.
 * You should have received a copy of the license along with this work.  If not, see <http://creativecommons.org/licenses/by-sa/4.0/>.
 */
namespace Tds.CommonBehaviours
{
    using UnityEngine;

    /// <summary>
    /// Behaviour which causes a game object to 'wobble' (sin over the transform's position)
    /// </summary>
    public class FloatingBehaviour : MonoBehaviour
    {
        /// <summary>
        /// Frequency of the wobble 
        /// </summary>
        public float _frequency;

        /// <summary>
        /// Amplitude ('height') of the wobble
        /// </summary>
        public float _amplitude;

        /// <summary>
        /// Vector to wobble along
        /// </summary>
        public Vector3 _vector;

        private Vector3 _offset = Vector3.zero;

        public void Start()
        {
            _offset = Vector3.zero;
        }

        public void Update()
        {
            _offset = Mathf.Sin(_frequency * Time.time) * _amplitude * _vector;
            transform.position += _offset;
        }
    }
}
