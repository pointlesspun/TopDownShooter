/*
 * TDS (c) 2018 by Another Pointless Pun
 * TDS is licensed under a Creative Commons Attribution-ShareAlike 4.0 International License.
 * You should have received a copy of the license along with this work.  If not, see <http://creativecommons.org/licenses/by-sa/4.0/>.
 */
namespace Tds.GameScripts
{
    using System.Collections.Generic;
    using UnityEngine;
    using Tds.CommonBehaviours;

    /// <summary>
    /// Component capturing the behaviour of a gamepoint with hitpoints. 
    /// </summary>
    public class Hitpoints : MonoBehaviour
    {
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
        private List<SpriteRendererColorInformation> _renderers = new List<SpriteRendererColorInformation>();

        /// <summary>
        /// Last time this gameobject took damage.
        /// </summary>
        private float _lastHitTime = 0;

        private ItemDropBehaviour _itemSpawner;

        public virtual void Start()
        {
            var levelScaling = GlobalGameState._levelScale;
            SpriteRendererExtensions.CollectSpriteRenderers(gameObject, _renderers);

            // scale the hitpoints per level
            _maxHitpoints = _maxHitpoints + _hitpointsScalePerLevel * levelScaling;
            _hitpoints = _hitpoints + _hitpointsScalePerLevel * levelScaling;

            var spawner = GameObject.FindGameObjectWithTag(GameTags.ItemSpawner);

            if (spawner != null)
            {
                _itemSpawner = spawner.GetComponent<ItemDropBehaviour>();
            }
        }

        public void Update()
        {
            // update the hit cue (coloring) if it applies
            if ( Time.time - _lastHitTime < _onHitDuration )
            {
                var value = ((Time.time - _lastHitTime) / _onHitDuration);

                SpriteRendererExtensions.LerpColor(_renderers, _onHitColor, value);
            }
        }

        /// <summary>
        /// Callback if the character picks up a health item
        /// </summary>
        /// <param name="amount"></param>
        public void OnPickupHealth(float amount)
        {
            _hitpoints = Mathf.Min(_maxHitpoints, _hitpoints + amount * _maxHitpoints);
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
                    if ( _itemSpawner != null)
                    {
                        _itemSpawner.OnCharacterDied(gameObject);
                    }

                    // no more hitpoints... so die
                    Destroy(gameObject);
                }
            }
        }
    }
}
