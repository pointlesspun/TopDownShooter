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
    /// Causes all spriterenders to be cycle through the colors defined in this behaviour
    /// </summary>
    public class ColorPulseBehaviour : MonoBehaviour
    {
        /// <summary>
        /// Colors to cycle through
        /// </summary>
        public Color[] _colors;

        /// <summary>
        /// Duration of the current color cycle
        /// </summary>
        public float[] _durations;

        private int _index;
        private float _currentColorCycleStart;

        /// <summary>
        /// Cached renderers which make up this game object so we can color them if the 
        /// gameobject takes damage.
        /// </summary>        
        private List<SpriteRendererColorInformation> _renderers = new List<SpriteRendererColorInformation>();

        public virtual void Start()
        {
            SpriteRendererExtensions.CollectSpriteRenderers(gameObject, _renderers);
            _index = 0;
            _currentColorCycleStart = Time.time;
        }

        public void Update()
        {
            var value = (Time.time - _currentColorCycleStart) / _durations[_index];
            var next = (_index + 1) % _colors.Length;
            SpriteRendererExtensions.LerpColor(_renderers, _colors[_index], _colors[next], Mathf.Min(1.0f, value));

            if (value >= 1.0f)
            {
                _index = next;
                _currentColorCycleStart = Time.time;
            }
        }
    }
}
