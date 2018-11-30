/*
 * TDS (c) 2018 by Another Pointless Pun
 * TDS is licensed under a Creative Commons Attribution-ShareAlike 4.0 International License.
 * You should have received a copy of the license along with this work.  If not, see <http://creativecommons.org/licenses/by-sa/4.0/>.
 */
namespace Tds.GameScripts
{
    using UnityEngine;

    public class RotatingBehaviour : MonoBehaviour
    {
        public Vector3 _axis;
        public float _degreesPerSecond;

        public void Update()
        {
            transform.rotation *= Quaternion.AngleAxis(_degreesPerSecond * Time.deltaTime, _axis);
        }
    }
}