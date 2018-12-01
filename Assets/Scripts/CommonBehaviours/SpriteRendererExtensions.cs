/*
 * TDS (c) 2018 by Another Pointless Pun
 * TDS is licensed under a Creative Commons Attribution-ShareAlike 4.0 International License.
 * You should have received a copy of the license along with this work.  If not, see <http://creativecommons.org/licenses/by-sa/4.0/>.
 */
namespace Tds.CommonBehaviours
{
    using System.Collections.Generic;
    using UnityEngine;

    /// <summary>
    /// Information about the color of a given sprite renderer.
    /// </summary>
    public class SpriteRendererColorInformation
    {
        public SpriteRenderer _renderer;

        /// <summary>
        /// Color with which the sprite renderer started.
        /// </summary>
        public Color _originalColor;
    }

    /// <summary>
    /// Utility functions for sprite rendering
    /// </summary>
    public static class SpriteRendererExtensions 
    {
        /// <summary>
        /// Recursively collects the sprite renderers in the given game object
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="result"></param>
        public static void CollectSpriteRenderers(GameObject obj, List<SpriteRendererColorInformation> result)
        {
            var renderer = obj.GetComponent<SpriteRenderer>();

            if (renderer != null)
            {
                result.Add(new SpriteRendererColorInformation()
                {
                    _originalColor = renderer.color,
                    _renderer = renderer
                });
            }

            for (int i = 0; i < obj.transform.childCount; i++)
            {
                CollectSpriteRenderers(obj.transform.GetChild(i).gameObject, result);
            }
        }

        /// <summary>
        /// Lerp the colors in the given sprite renders from the given color to the renderer's original color
        /// </summary>
        /// <param name="renderers"></param>
        /// <param name="from"></param>
        /// <param name="value"></param>
        public static void LerpColor(List<SpriteRendererColorInformation> renderers, Color from,  float value)
        {
            foreach (var info in renderers)
            {
                info._renderer.color = Color.Lerp(from, info._originalColor, value);
            }
        }

        /// <summary>
        /// Lerp the colors in the given sprite renders from the given from-color to the given to-color
        /// </summary>
        public static void LerpColor(List<SpriteRendererColorInformation> renderers, Color from, Color to, float value)
        {
            foreach (var info in renderers)
            {
                info._renderer.color = Color.Lerp(from, to, value);
            }
        }
    }
}
