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

        public static Vector2Int[] GetTouchPoints(RectInt a, RectInt b)
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

    }
}
