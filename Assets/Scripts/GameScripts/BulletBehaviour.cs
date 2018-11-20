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
        /// Potential damage caused to the colliding party
        /// </summary>
        public float _damage = 1;

        /// <summary>
        /// No damage will be triggered on objects with this tag.
        /// </summary>
        public string _friendlyTag = GameTags.Player;

        /// <summary>
        /// Range in Unity units, after which the gameobject is destroyed. If set to -1 
        /// the bullet has no maxrange.
        /// </summary>
        public float _maxRange = -1;

        /// <summary>
        /// Reference to the rigid body
        /// </summary>
        private Rigidbody2D _body;

        /// <summary>
        /// Time this bullet was started, used to track the lifetime
        /// </summary>
        private float _startTime;

        /// <summary>
        /// Point at which the was spawned, used to calculate the range check.
        /// </summary>
        private Vector3 _startPoint;

        void Start()
        {
            _body = GetComponent<Rigidbody2D>();
            _body.velocity = _direction * _velocity;
            _startTime = Time.time;
            _startPoint = transform.position;
        }

        void Update()
        {
            // bullet only tracks lifetime, collisions are handled in the 'CollisionDamage' component.
            if (Time.time - _startTime > _lifetime)
            {
                Destroy(gameObject);
            }
            // check max range condition
            else if (_maxRange > 0 && ((transform.position - _startPoint).sqrMagnitude > _maxRange * _maxRange))
            {
                Destroy(gameObject);
            }
        }

        void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.gameObject.tag != _friendlyTag)
            {
                collision.gameObject.SendMessage(MessageNames.OnDamage, _damage, SendMessageOptions.RequireReceiver);
            }

            Destroy(gameObject);
        }
    }
}
