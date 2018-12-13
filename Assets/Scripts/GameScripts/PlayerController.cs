/*
 * TDS (c) 2018 by Another Pointless Pun
 * TDS is licensed under a Creative Commons Attribution-ShareAlike 4.0 International License.
 * You should have received a copy of the license along with this work.  If not, see <http://creativecommons.org/licenses/by-sa/4.0/>.
 */
namespace Tds.GameScripts
{
    using System.Collections.Generic;
    using Tds.Util;
    using UnityEngine;
    using UnityEngine.SceneManagement;

    /// <summary>
    /// Behaviour handling the player's input
    /// </summary>
    public class PlayerController : MonoBehaviour
    {
        /// <summary>
        /// Movement speed reduction when the player is not moving
        /// </summary>
        public float _velocityDampening = 8;

        /// <summary>
        /// Max movement speed of the player
        /// </summary>
        public float _maxVelocity = 4;

        /// <summary>
        /// Min movement speed, if the speed drops below this value, the velocity will be set to 0.
        /// </summary>
        public float _minVelocity = 0.0005f;

        /// <summary>
        /// How quickly the player picks up speed once moving.
        /// </summary>
        public float _velocityIncrease = 32;

        /// <summary>
        /// Object holding the animation controller
        /// </summary>
        public GameObject _animatorControllerObject;

        /// <summary>
        /// Speed at which the character is considered idle.
        /// </summary>
        public float _idleSpeed = 0.4f;

        /// <summary>
        /// If set to true, the player is being controlled by another entity
        /// </summary>
        public bool _isPuppet = false;

        /// <summary>
        /// Reference to the body
        /// </summary>
        private Rigidbody2D _body;

        /// <summary>
        /// Reference to the weapon
        /// </summary>
        private List<WeaponBase> _weapons = new List<WeaponBase>();
        private int _currentWeaponIndex = -1;

        /// <summary>
        /// Reference to the camera
        /// </summary>
        private Camera _camera;

        /// <summary>
        /// Cached object to send attack descriptions
        /// </summary>
        private AttackParameters _attackDescription = new AttackParameters();

        /// <summary>
        /// Cached animator
        /// </summary>
        private Animator _animator;

        void Start()
        {
            Contract.RequiresComponent<Rigidbody2D>(gameObject, "The player is required to have a RigidBody2D component.");
            Contract.Requires(_animatorControllerObject != null, "The player object is required to have the _animatorControllerObject parameter set.");

            _body = GetComponent<Rigidbody2D>();
            _animator = _animatorControllerObject.GetComponent<Animator>();

            _camera = Camera.main;
            _weapons = new List<WeaponBase>(GetComponentsInChildren<WeaponBase>());

            _currentWeaponIndex = SelectWeapon();


            Contract.Requires(_camera != null, "The player object is required to be able to access the camera.");
        }

        /// <summary>
        /// Callback when the player gets enabled, starts listening to sceneloaded events
        /// </summary>
        public void OnEnable()
        {
            SceneManager.sceneLoaded += this.OnLoadCallback;
        }

        /// <summary>
        /// Callback when the player gets disabled, stops listening to sceneloaded events
        /// </summary>
        public void OnDisable()
        {
            SceneManager.sceneLoaded -= this.OnLoadCallback;
        }

        public void OnLoadCallback(Scene scene, LoadSceneMode sceneMode)
        {
            _camera = Camera.main;
            Contract.Requires(_camera != null, "The player object is required to be able to access the camera.");
        }

        public void Update()
        {
            if (_currentWeaponIndex == -1)
            {
                _currentWeaponIndex = SelectWeapon();
            }

            if (_isPuppet)
            {
                UpdatePuppetState();
            }
            else
            {
                UpdatePlayerControlledState();
            }
        }

        /// <summary>
        /// Update the state where the player is in control of the character
        /// </summary>
        private void UpdatePlayerControlledState()
        {          
            var velocity = UpdateVelocity();
            var cursorPosition = InputUtil.GetCursorPosition(_camera);

            var animationState = AnimationStateDecisionTree.GetAnimationState(transform.position, cursorPosition, velocity, _idleSpeed);

            if (_currentWeaponIndex != -1)
            {
                UpdateAttack(cursorPosition, _weapons[_currentWeaponIndex]);
            }

            _animator.SetInteger(AnimatorParameterNames.AnimationState, animationState);
        }

        /// <summary>
        /// Update the state in which the player gets controlled  by another entity than the player
        /// </summary>
        private void UpdatePuppetState()
        {           
            var velocity = new Vector3(_body.velocity.x, _body.velocity.y, 0);

            // in puppet mode the player's 'cursor' is in the direction the player is walking            
            var cursorPosition = transform.position + velocity;

            var animationState = AnimationStateDecisionTree.GetAnimationState(transform.position, cursorPosition, velocity, _idleSpeed);
            
            _animator.SetInteger(AnimatorParameterNames.AnimationState, animationState);
        }

        /// <summary>
        /// Callback from when a weapon gets destroyed
        /// </summary>
        public void OnWeaponDestroyed()
        { 
            // assumotion is that the player is holding the weapon
            _weapons.RemoveAt(_currentWeaponIndex);
            _currentWeaponIndex = -1;
        }


        /// <summary>
        /// Callback when an item is picked up
        /// </summary>
        /// <param name="prefab"></param>
        public void OnPickupItem(GameObject item)
        {
            var component = item.GetComponent<WeaponBase>();

            // right now we only deal with weapons
            if (component != null)
            {
                // check if the player is already carrying the same weapon
                var existing = _weapons.Find((w) => component.name == w.name);

                if (existing != null)
                {
                    // same weapon is already in the inventory, merge the two
                    existing.Merge(component);
                }
                else
                {
                    // no existing item, add the item to the player
                    item.transform.parent = transform;

                    _weapons.Add(component);

                    // if the weapon is better, equip it
                    if (_currentWeaponIndex == -1 || component._priority > _weapons[_currentWeaponIndex]._priority)
                    {
                        _currentWeaponIndex = _weapons.Count - 1;
                    }
                }
            } else
            {
                // reminder that we need to have code 
                Debug.LogWarning("Unknown item picked up: " + item.name + "::" + component.name);
            }
        }

        /// <summary>
        /// Sets the movement direction of the player
        /// </summary>
        /// <param name="velocity"></param>
        public void SetVelocity(Vector3 velocity)
        {
            _body.velocity = velocity;
        }

        /// <summary>
        /// Update the velocity / movement of the player
        /// </summary>
        /// <returns></returns>
        private Vector3 UpdateVelocity()
        {
            var velocity = _body.velocity;

            // reduce the player speed
            velocity *= 1.0f - (_velocityDampening * Time.deltaTime);

            velocity += Vector2.up * Input.GetAxis(InputNames.VerticalAxis) * _velocityIncrease * Time.deltaTime;
            velocity += Vector2.right * Input.GetAxis(InputNames.HorizontalAxis) * _velocityIncrease * Time.deltaTime;

            var direction = velocity.normalized;
            var magnitude = velocity.magnitude;

            // cap velocity
            if (magnitude > _maxVelocity)
            {
                magnitude = _maxVelocity;
            }
            else if (magnitude < _minVelocity)
            {
                magnitude = 0;
            }

            _body.velocity = magnitude * direction;

            return _body.velocity;
        }

        /*private Vector3 GetCursorPosition()
        {
            var mouseScreenPosition = Input.mousePosition;
            var result = _camera.ScreenToWorldPoint(new Vector3(mouseScreenPosition.x, mouseScreenPosition.y, _camera.nearClipPlane));

            result.z = 0;

            return result;
        }*/

        private void UpdateAttack(Vector3 cursorPosition, WeaponBase weapon)
        {
            var attackDirection = (cursorPosition - transform.position).normalized;
            weapon.transform.position = transform.position + attackDirection * weapon._offsetFromOwner;

            if (Input.GetButton(InputNames.Fire1) && _weapons != null && weapon.IsCooldownOver())
            {
                _attackDescription._direction = attackDirection;
                weapon.Attack(_attackDescription);
            }
        }

        /// <summary>
        /// Simple weapon select implementation, will change in forthcoming builds
        /// </summary>
        /// <returns></returns>
        private int SelectWeapon()
        {
            var bestPriority = 0;
            int bestWeapon = -1;
                 
            WeaponBase weapon = null;

            for (int i = 0; i < _weapons.Count; ++i)
            {
                weapon = _weapons[i];

                if (weapon != null && (bestWeapon == -1|| weapon._priority > bestPriority))
                {
                    bestWeapon = i;
                    bestPriority = weapon._priority;
                }
            }

            return bestWeapon;
        }
    }
}
