/*
 * TDS (c) 2018 by Another Pointless Pun
 * TDS is licensed under a Creative Commons Attribution-ShareAlike 4.0 International License.
 * You should have received a copy of the license along with this work.  If not, see <http://creativecommons.org/licenses/by-sa/4.0/>.
 */
using UnityEngine;

namespace Tds.Util
{
    public static class MathUtil
    {
        public static int TriangleWave(int x, int period)
        {
            return period == 0 ? 0 : period - Mathf.Abs(x % (2 * period) - period);
        }

        public static float TriangleWave(float x, float period)
        {
            return period == 0 ? 0 : period - Mathf.Abs(x % (2 * period) - period);
        }
    }
}
