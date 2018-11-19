/*
 * TDS (c) 2018 by Another Pointless Pun
 * TDS is licensed under a Creative Commons Attribution-ShareAlike 4.0 International License.
 * You should have received a copy of the license along with this work.  If not, see <http://creativecommons.org/licenses/by-sa/4.0/>.
 */
namespace Tds.GameScripts
{
    using UnityEngine;

    /// <summary>
    /// Behaviour of a bullet in the game
    /// </summary>
    public class BulletBehaviour : MonoBehaviour
    {
        /// <summary>
        /// Field indicating how long the bullet lives in seconds
        /// </summary>
        public float _lifetime;

        /// <summary>
        /// Direction of the bullet
        /// </summary>
        public Vector3 _direction;

        /// <summary>
        /// Velocity of the bullet in 'unity units' per second
        /// </summary>
        public float _velocity;

        /// <summary>
        /// Reference to the rigid body
        /// </summary>
        private Rigidbody2D _body;

        /// <summary>
        /// Time this bullet was started, used to track the lifetime
        /// </summary>
        private float _startTime;

        void Start()
        {
            _body = GetComponent<Rigidbody2D>();
            _body.velocity = _direction * _velocity;
            _startTime = Time.time;
        }

        void Update()
        {
            // bullet only tracks lifetime, collisions are handled in the 'CollisionDamage' component.
            if (Time.time - _startTime > _lifetime)
            {
                Destroy(gameObject);
            }
        }
    }
}
