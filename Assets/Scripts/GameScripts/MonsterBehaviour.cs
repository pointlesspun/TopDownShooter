/*
 * TDS (c) 2018 by Another Pointless Pun
 * TDS is licensed under a Creative Commons Attribution-ShareAlike 4.0 International License.
 * You should have received a copy of the license along with this work.  If not, see <http://creativecommons.org/licenses/by-sa/4.0/>.
 */
namespace Tds.GameScripts
{
    using UnityEngine;

    /// <summary>
    /// Behaviour driving the "monsters" and their AI in the game. 
    /// </summary>    
    public class MonsterBehaviour : MonoBehaviour
    {
        /// Max movement speed 
        public float _maxSpeed = 2.0f;

        /// Body controlling the physics
        private Rigidbody2D _body;

        /// Weapon the monster is holding
        private WeaponBase _weapon;

        /// Target of the monsters 
        private GameObject _player;

        /// Cached object containing the information required to attack
        private AttackParameters _attackDescription = new AttackParameters();

        /// <summary>
        /// Object holding the animation controller
        /// </summary>
        public GameObject _animatorControllerObject;

        /// <summary>
        /// Scaling of the movement speed per level (if set to 0 no scaling will apply). Eg. this will
        /// make monsters faster as the user goes through the levels. 
        /// </summary>
        public float _maxSpeedScaling = 0;

        /// <summary>
        /// Cached animator
        /// </summary>
        private Animator _animator;


        public void Start()
        {
            _player = GlobalGameState._playerObject;

            _body = GetComponent<Rigidbody2D>();
            _weapon = GetComponent<WeaponBase>();

            _animator = _animatorControllerObject.GetComponent<Animator>();

            // scale movement speed off the level
            _maxSpeed = _maxSpeed + GlobalGameState._levelScale * _maxSpeedScaling;

         
        }

        public void Update()
        {
            var animationState = AnimationState.MovingBackwardFacingRight;

            _body.velocity = Vector3.zero;

            // only do something if the player is alive
            if (_player != null && _player.activeInHierarchy)
            {
                // select a weapon if no weapon is being held
                if (_weapon == null)
                {
                    _weapon = SelectWeapon();
                }

                // if a weapon is armed and it can fire (this will cause the monster to 
                // stop and fire)
                if (_weapon && _weapon.IsCooldownOver())
                {
                    AimWeapon(_weapon, _player);

                    // if in range attack the player
                    if (_weapon.IsInRange(_player))
                    {
                        AttackPlayer();
                    }
                    else
                    {
                        // if not move towards the player
                        var movementDirection = (_player.transform.position - transform.position);
                        movementDirection.z = 0;
                        movementDirection.Normalize();
                        _body.velocity = movementDirection * _maxSpeed;
                    }
                }

                animationState = AnimationStateDecisionTree.GetAnimationState(transform.position, _player.transform.position, _body.velocity, 0.4f);
            }

            _animator.SetInteger(AnimatorParameterNames.AnimationState, animationState);
        }

        private void AttackPlayer()
        {
            Vector3 attackDirection = (_player.transform.position - transform.position);
            attackDirection.z = 0;
            _attackDescription._target = _player;
            _attackDescription._direction = attackDirection.normalized;
            _weapon.Attack(_attackDescription);
        }

        /// <summary>
        /// Simple selection placeholder which just uses the first weapon found in the game object's hierarchy
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

        private void AimWeapon(WeaponBase weapon, GameObject target)
        {
            var direction = (target.transform.position - transform.position);

            direction.z = 0;
            direction.Normalize();

            weapon.gameObject.transform.position = transform.position + direction * weapon._offsetFromOwner;
        }
    }
}
