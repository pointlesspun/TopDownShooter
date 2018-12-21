/*
 * TDS (c) 2018 by Another Pointless Pun
 * TDS is licensed under a Creative Commons Attribution-ShareAlike 4.0 International License.
 * You should have received a copy of the license along with this work.  If not, see <http://creativecommons.org/licenses/by-sa/4.0/>.
 */
namespace Tds.Util
{
    using UnityEngine;

    public struct Line2D
    {
        public Vector2 from;
        public Vector2 to;

        public float Length
        {
            get
            {
                return (to - from).magnitude;
            }
        }

        public Vector2 Interpolation(float offset)
        {
            var direction = to - from;
            var length = direction.magnitude;
            
            return from + direction * offset;
        }

        public Line2D Cross(float offset = 0.5f)
        {
            var direction = to - from;
            var length = direction.magnitude;
            var unit = direction.normalized;

            var point = from + direction * offset;

            return new Line2D()
            {
                from = point,
                to = point + new Vector2(unit.y, -unit.x)
            };
        } 

    }
}
