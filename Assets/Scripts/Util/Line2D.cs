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

        /// <summary>
        /// Returns a point interpolated over this line 
        /// </summary>
        /// <param name="offset">A value between 0.0 (from) to 1.0 (to) </param>
        /// <returns></returns>
        public Vector2 Interpolation(float offset)
        {
            return from + Direction * offset;
        }

        /// <summary>
        /// Returns the line representing a line orthogonal to this line starting at the interpolated point
        /// </summary>
        /// <param name="offset"></param>
        /// <returns></returns>
        public Line2D Cross(float offset = 0.5f)
        {
            var direction = Direction;
            var length = direction.magnitude;
            var unit = direction.normalized;

            var point = from + direction * offset;

            return new Line2D()
            {
                from = point,
                to = point + new Vector2(unit.y, -unit.x)
            };
        } 

        /// <summary>
        /// Returns the direction vector of this line
        /// </summary>
        public Vector2 Direction
        {
            get
            {
                return to - from; 
            }
        }

        /// <summary>
        /// Creates a randomized version between from and to points.
        /// </summary>
        /// <param name="minLength"></param>
        /// <param name="maxLength"></param>
        /// <returns></returns>
        public Line2D Randomized(float minLength, float maxLength, float fromConstraint = 0, float toConstraint = 0)
        {
            var lineLength = Length;

            var lineMinLength = Mathf.Max(minLength, fromConstraint);
            var lineMaxLength = Mathf.Min(maxLength, lineLength - (minLength + toConstraint));

            var length = Random.Range(lineMinLength, lineMaxLength);
            var offset = Random.Range(fromConstraint/ lineLength, 1.0f - (length/lineLength));

            var startPoint = Interpolation(offset);

            return new Line2D()
            {
                from = startPoint,
                to = startPoint + Direction.normalized * length
            };
        }

        /// <summary>
        /// Returns a version value where the from is rounded 
        /// </summary>
        public Line2D Snap()
        {
            return new Line2D()
            {
                from = new Vector2(Mathf.Round(from.x), Mathf.Round(from.y)),
                to = new Vector2(Mathf.Round(to.x), Mathf.Round(to.y))
            };
        }

        public Line2D Translate( float x, float y )
        {
            return new Line2D()
            {
                from = new Vector2(from.x + x, from.y + y),
                to = new Vector2(to.x + x, to.y + y)
            };
        }

        public override string ToString()
        {
            return "from " + from + " to " + to;
        }
    }
}
