/*
 * TDS (c) 2018 by AnrectB Pointless Pun
 * TDS is licensed under a Creative Commons Attribution-ShareAlike 4.0 International License.
 * You should have received a copy of the license along with this work.  If not, see <http://creativecommons.org/licenses/by-sa/4.0/>.
 */
namespace Tds.Util
{
    using UnityEngine;

    public static class RectUtil
    {
        /// <summary>
        /// Tests if the two rects do not overlap or touch
        /// </summary>
        /// <param name="rectA"></param>
        /// <param name="rectB"></param>
        /// <returns></returns>
        public static bool AreDisconnected(RectInt rectA, RectInt rectB)
        {
            var a = (rectA.min.x + rectA.width) - rectB.min.x;
            var b = (rectB.min.x + rectB.width) - rectA.min.x;

            var c = (rectA.min.y + rectA.height) - rectB.min.y;
            var d = (rectB.min.y + rectB.height) - rectA.min.y;

            var e = (a == 0 ? 1 : 0)
                + (b == 0 ? 1 : 0)
                + (c == 0 ? 1 : 0)
                + (d == 0 ? 1 : 0);

            return (a < 0 || b < 0)
                   || (c < 0 || d < 0)
                   || e >= 2;
        }

        /// <summary>
        /// Gets an intersection line between two rectangles. There are three possible outcomes:
        /// 
        /// * v1.x == v2.x or v1.y == v2.y, rectangles touch on a side 
        /// * v1.x > v2.x or v1.y > v2.y, rectangles are disjunct
        /// * none of the above applies and v1.x != v2.x or v1.y != v2.y, rectangles intersect 
        /// 
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static Vector2Int[] GetIntersection(RectInt a, RectInt b)
        {
            var x1 = Mathf.Max(a.min.x, b.min.x);
            var x2 = Mathf.Min(a.max.x, b.max.x);

            var y1 = Mathf.Max(a.min.y, b.min.y);
            var y2 = Mathf.Min(a.max.y, b.max.y);

            return new Vector2Int[]
            {
                new Vector2Int(x1, y1),
                new Vector2Int(x2, y2),
            };
        }

        /// <summary>
        /// Clamp the point against the rect, shrinking the rect with the given delta width / height.
        /// </summary>
        /// <param name="rect"></param>
        /// <param name="point"></param>
        /// <param name="deltaWidth"></param>
        /// <param name="deltaHeight"></param>
        /// <returns></returns>
        public static Vector2 Clamp(RectInt rect, Vector2 point, float deltaWidth = 0, float deltaHeight = 0)
        {
            return new Vector2(Mathf.Clamp(point.x, rect.min.x + deltaWidth, rect.max.x - deltaWidth),
                                                        Mathf.Clamp(point.y, rect.min.y + deltaWidth, rect.max.y - deltaWidth));
        }

        /// <summary>
        /// Creates a rect out bounding two circles where the circles are defined by a point and a given radius.
        /// </summary>
        /// <param name="p1">center of circle 1</param>
        /// <param name="p2">center of circle 2</param>
        /// <param name="radius">radius of both circles</param>
        /// <returns></returns>
        public static Rect CreateBoundingBox(Vector2 p1, Vector2 p2, float radius )
        {
            return new Rect()
            {
                min = new Vector2(Mathf.Min(p1.x, p2.x) - radius, Mathf.Min(p1.y, p2.y) - radius),
                max = new Vector2(Mathf.Max(p1.x, p2.x) + radius, Mathf.Max(p1.y, p2.y) + radius)
            };
        }
    }
}
