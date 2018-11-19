/*
 * TDS (c) 2018 by Another Pointless Pun
 * TDS is licensed under a Creative Commons Attribution-ShareAlike 4.0 International License.
 * You should have received a copy of the license along with this work.  If not, see <http://creativecommons.org/licenses/by-sa/4.0/>.
 */
namespace Tds.GameScripts
{
    using UnityEngine;

    /// <summary>
    /// Behaviour describing what happens on when the game object raises a collision enter
    /// </summary>
    public class CollisionDamage : MonoBehaviour
    {
        /// <summary>
        /// Potential damage caused to the colliding party
        /// </summary>
        public int _damage = 1;

        /// <summary>
        /// Damage caused to the gameobject associated with this behaviour
        /// </summary>
        public int _damageToSelf = 0;

        /// <summary>
        /// No damage will be triggered on objects with this tag.
        /// </summary>
        public string _friendlyTag = GameTags.Player;

        /// <summary>
        /// If set to true the gameobject will be destroyed when a collision is flagged
        /// </summary>
        public bool _destroyOnCollision = false;

        void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.gameObject.tag != _friendlyTag)
            {
                collision.gameObject.SendMessage(MessageNames.OnDamage, _damage, SendMessageOptions.RequireReceiver);

                if (_damageToSelf != 0)
                {
                    gameObject.SendMessage(MessageNames.OnDamage, _damage, SendMessageOptions.RequireReceiver);
                }
            }

            if (_destroyOnCollision)
            {
                Destroy(gameObject);
            }
        }
    }
}
