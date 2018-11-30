/*
 * TDS (c) 2018 by Another Pointless Pun
 * TDS is licensed under a Creative Commons Attribution-ShareAlike 4.0 International License.
 * You should have received a copy of the license along with this work.  If not, see <http://creativecommons.org/licenses/by-sa/4.0/>.
 */
namespace Tds.GameScripts
{
    using UnityEngine;

    public class FloatingBehaviour : MonoBehaviour
    {
        public float _frequency;
        public float _amplitude;

        public Vector3 _vector;

        private Vector3 _offset = Vector3.zero;

        public void Start()
        {
            _offset = Vector3.zero;
        }

        public void Update()
        {
            var position = transform.position - _offset;

            _offset = Mathf.Sin(_frequency * Time.time) * _amplitude * _vector;

            transform.position += _offset;
        }
    }
}
