/*
 * TDS (c) 2018 by Another Pointless Pun
 * TDS is licensed under a Creative Commons Attribution-ShareAlike 4.0 International License.
 * You should have received a copy of the license along with this work.  If not, see <http://creativecommons.org/licenses/by-sa/4.0/>.
 */
namespace Tds.GameScripts
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    public class SpriteRendereColorInformation
    {
        public SpriteRenderer _renderer;
        public Color _originalColor;
    }

    /// <summary>
    /// </summary>
    public static class SpriteRendererExtensions 
    {
        public static void CollectSpriteRenderers(GameObject obj, List<SpriteRendereColorInformation> result)
        {
            var renderer = obj.GetComponent<SpriteRenderer>();

            if (renderer != null)
            {
                result.Add(new SpriteRendereColorInformation()
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

        public static void LerpColor(List<SpriteRendereColorInformation> renderers, Color from,  float value)
        {
            foreach (var info in renderers)
            {
                info._renderer.color = Color.Lerp(from, info._originalColor, value);
            }
        }

        public static void LerpColor(List<SpriteRendereColorInformation> renderers, Color from, Color to, float value)
        {
            foreach (var info in renderers)
            {
                info._renderer.color = Color.Lerp(from, to, value);
            }
        }
    }
}
