/*
 * TDS (c) 2018 by Another Pointless Pun
 * TDS is licensed under a Creative Commons Attribution-ShareAlike 4.0 International License.
 * You should have received a copy of the license along with this work.  If not, see <http://creativecommons.org/licenses/by-sa/4.0/>.
 */
namespace Tds.GameScripts
{
    using UnityEngine;
    using UnityEngine.UI;

    /// <summary>
    /// Behaviour updating a healthbar for the tracked target.
    /// </summary>
    public class HealthbarBehaviour : MonoBehaviour {

        /// <summary>
        /// Target object for which to show the hitpoints 
        /// </summary>
        public GameObject _target;

        /// <summary>
        /// Object holding the image for the healthbar
        /// </summary>
        public GameObject _hitpointsHealthbarObject;

        /// <summary>
        /// Max length of the healthbar, should be the same as the healthbar scale in the game
        /// </summary>
        public float _maxHitbarXScale;

        /// <summary>
        /// Reference to the hitpoints behaviour of the target
        /// </summary>
        private Hitpoints _hitpointBehaviour;

        /// <summary>
        /// Reference to the image representing the healthbar
        /// </summary>
        private Image     _hitpointsBar;


        public void Start() {

            SetTarget(_target);
        }

        /// <summary>
        /// Sets the target of which to follow the healthbar
        /// </summary>
        /// <param name="target"></param>
        public void SetTarget(GameObject target)
        {
            Contract.Requires(target != null, "Could not resolve hitpoints behaviour on null target");

            _target = target;

            _hitpointBehaviour = _target.GetComponent<Hitpoints>();

            Contract.Requires(_hitpointBehaviour != null, "Could not resolve hitpoints behaviour on " + _target.name);

            Contract.Requires(_hitpointsHealthbarObject != null, "Cannot get healthbar from null object.");

            _hitpointsBar = _hitpointsHealthbarObject.GetComponent<Image>();

            Contract.Requires(_hitpointsBar != null, "Cannot get resolve hitpoints bar image.");
        }

        void Update() {
            var scale = _hitpointsBar.rectTransform.localScale;
            scale.x = _maxHitbarXScale * Mathf.Max(0, (_hitpointBehaviour._hitpoints / _hitpointBehaviour._maxHitpoints));
            _hitpointsBar.rectTransform.localScale = scale;  
        }
    }
}