/*
 * TDS (c) 2018 by Another Pointless Pun
 * TDS is licensed under a Creative Commons Attribution-ShareAlike 4.0 International License.
 * You should have received a copy of the license along with this work.  If not, see <http://creativecommons.org/licenses/by-sa/4.0/>.
 */
namespace Tds.Util
{
    using UnityEngine;

    /// <summary>
    /// Draws a cross at the position of the associated game object
    /// </summary>
    public class DrawCrossGizmo : MonoBehaviour
    {
        public Vector3 _offset;
        public float _scale = 0.3f;

        public void OnDrawGizmos()
        {
            Gizmos.DrawLine(_offset + new Vector3(-1, -1) * _scale, _offset + new Vector3(1, 1) * _scale);
            Gizmos.DrawLine(_offset + new Vector3(-1, 1) * _scale, _offset + new Vector3(1, -1) * _scale);
        }
    }
}