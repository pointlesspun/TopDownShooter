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

    /// <summary>
    /// Component capturing the behaviour of a gamepoint with hitpoints. 
    /// </summary>
    public class Hitpoints : MonoBehaviour
    {
        private class SpriteRendereColorInformation
        {
            public SpriteRenderer _renderer;
            public Color          _originalColor;
        }

        /// <summary>
        /// Max hitpoints, current hitpoints should never exceed this
        /// </summary>
        public float _maxHitpoints = 5;

        /// <summary>
        /// Flag when set to true will ignore all damage taken
        /// </summary>
        public bool _isInvulnerable = false;

        /// <summary>
        /// Current hitpoints, if this drops to 0 or below, the gameojbect associated with this 
        /// </summary>
        public float _hitpoints = 5;

        /// <summary>
        /// Color applied when the character loses hitpoints
        /// </summary>
        public Color _onHitColor = Color.white;

        /// <summary>
        /// Length of the on hit color animation
        /// </summary>
        public float _onHitDuration = 0.25f;

        /// <summary>
        /// Scaling which allows enemies to gain more hit points as the elvesl increase
        /// </summary>
        public float _hitpointsScalePerLevel = 0;

        /// <summary>
        /// Cached renderers which make up this game object so we can color them if the 
        /// gameobject takes damage.
        /// </summary>        
        private List<SpriteRendereColorInformation> _renderers = new List<SpriteRendereColorInformation>();

        /// <summary>
        /// Last time this gameobject took damage.
        /// </summary>
        private float _lastHitTime = 0;

        public virtual void Start()
        {
            var levelScaling = GlobalGameState._levelScale;
            CollectSpriteRenderers(gameObject, _renderers);

            // scale the hitpoints per level
            _maxHitpoints = _maxHitpoints + _hitpointsScalePerLevel * levelScaling;
            _hitpoints = _hitpoints + _hitpointsScalePerLevel * levelScaling;
        }

        public void Update()
        {
            // update the hit cue if it applies
            if ( Time.time - _lastHitTime < _onHitDuration )
            {
                var value = ((Time.time - _lastHitTime) / _onHitDuration);

                foreach ( var info in _renderers)
                {
                    info._renderer.color = Color.Lerp(_onHitColor, info._originalColor, value);
                }
            }
        }

        /// <summary>
        /// OnDamage reduce the hitpoints and give a visual cue
        /// </summary>
        /// <param name="damage"></param>
        public void OnDamage(float damage)
        {
            if (!_isInvulnerable)
            {
                _hitpoints -= damage;
                _lastHitTime = Time.time;

                if (_hitpoints <= 0)
                {
                    // no more hitpoints... so die
                    Destroy(gameObject);
                }
            }
        }

        private void CollectSpriteRenderers(GameObject obj, List<SpriteRendereColorInformation> result)
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

            for ( int i = 0; i < obj.transform.childCount; i++)
            {
                CollectSpriteRenderers(obj.transform.GetChild(i).gameObject, result);
            }
        }
    }
}
