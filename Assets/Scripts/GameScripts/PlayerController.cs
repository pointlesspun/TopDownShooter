﻿/*
 * TDS (c) 2018 by Another Pointless Pun
 * TDS is licensed under a Creative Commons Attribution-ShareAlike 4.0 International License.
 * You should have received a copy of the license along with this work.  If not, see <http://creativecommons.org/licenses/by-sa/4.0/>.
 */
namespace Tds.GameScripts
{
    using UnityEngine;

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
        /// Reference to the body
        /// </summary>
        private Rigidbody2D _body;

        /// <summary>
        /// Reference to the weapon
        /// </summary>
        private WeaponBase _weapon;

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
            _weapon = SelectWeapon();
        }

        void Update()
        {
            var cursorPosition = GetCursorPosition();
            var velocity = UpdateVelocity();
            var animationState = AnimationStateDecisionTree.GetAnimationState(transform.position, cursorPosition, velocity, _idleSpeed);

            UpdateAttack(cursorPosition);

            _animator.SetInteger(AnimatorParameterNames.AnimationState, animationState);
        }

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

        private Vector3 GetCursorPosition()
        {
            var mouseScreenPosition = Input.mousePosition;
            var result = _camera.ScreenToWorldPoint(new Vector3(mouseScreenPosition.x, mouseScreenPosition.y, _camera.nearClipPlane));

            result.z = 0;

            return result;
        }

        private void UpdateAttack(Vector3 cursorPosition)
        {

            var attackDirection = (cursorPosition - transform.position).normalized;
            _weapon.transform.position = transform.position + attackDirection * _weapon._offsetFromOwner;

            if (Input.GetButton(InputNames.Fire1) && _weapon != null && _weapon.IsCooldownOver())
            {
                _attackDescription._direction = attackDirection;
                _weapon.Attack(_attackDescription);
            }
        }


        /// <summary>
        /// Simple weapon select implementation, will change in forthcoming builds
        /// </summary>
        /// <returns></returns>
        private WeaponBase SelectWeapon()
        {
            WeaponBase result = null;

            for (int i = 0; i < transform.childCount; ++i)
            {
                var child = transform.GetChild(i).gameObject;

                if (child.tag == GameTags.Weapon)
                {
                    result = child.GetComponent<WeaponBase>();

                    if (result != null)
                    {
                        break;
                    }
                }
            }

            return result;
        }
    }
}
