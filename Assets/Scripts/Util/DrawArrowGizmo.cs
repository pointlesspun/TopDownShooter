/*
 * TDS (c) 2018 by Another Pointless Pun
 * TDS is licensed under a Creative Commons Attribution-ShareAlike 4.0 International License.
 * You should have received a copy of the license along with this work.  If not, see <http://creativecommons.org/licenses/by-sa/4.0/>.
 */
namespace Tds.Util
{
    using UnityEngine;

    /// <summary>
    /// Gizmo which draws an arrow using lines
    /// </summary>
    public class DrawArrowGizmo : MonoBehaviour
    {
        public Vector3 _offset;
        public float _scale = 0.25f;

        public void OnDrawGizmos()
        {
            var basePoint = transform.position + _offset;
            var to = transform.rotation * (new Vector3(0, 0) * _scale);

            Gizmos.DrawLine(transform.rotation * (new Vector3(-1, -1) * _scale) + basePoint, to + basePoint);
            Gizmos.DrawLine(transform.rotation * (new Vector3(-1, 1) * _scale) + basePoint, to + basePoint);
            Gizmos.DrawLine(transform.rotation * (new Vector3(-2, 0) * _scale) + basePoint, to + basePoint);
        }
    }

}