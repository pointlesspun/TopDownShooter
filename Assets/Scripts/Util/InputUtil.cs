/*
 * TDS (c) 2018 by Another Pointless Pun
 * TDS is licensed under a Creative Commons Attribution-ShareAlike 4.0 International License.
 * You should have received a copy of the license along with this work.  If not, see <http://creativecommons.org/licenses/by-sa/4.0/>.
 */
namespace Tds.Util
{
    using UnityEngine;

    public static class InputUtil
    {
        public static Vector3 GetCursorPosition(Camera camera = null)
        {
            if (camera == null)
            {
                camera = Camera.main;
            }

            var mouseScreenPosition = Input.mousePosition;
            var result = camera.ScreenToWorldPoint(new Vector3(mouseScreenPosition.x, mouseScreenPosition.y, camera.nearClipPlane));

            result.z = 0;

            return result;
        }
    }
}
