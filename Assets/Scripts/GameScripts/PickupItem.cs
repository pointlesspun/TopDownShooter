/*
 * TDS (c) 2018 by Another Pointless Pun
 * TDS is licensed under a Creative Commons Attribution-ShareAlike 4.0 International License.
 * You should have received a copy of the license along with this work.  If not, see <http://creativecommons.org/licenses/by-sa/4.0/>.
 */
namespace Tds.GameScripts
{
    using UnityEngine;

    /// <summary>
    /// Describes what is being picked up. 
    /// </summary>
    public enum ItemCategory
    {
        Item,
        Health
    }

    /// <summary>
    /// Behaviour for items which can be picked up by a player
    /// </summary>
    public class PickupItem : MonoBehaviour
    {
        public ItemCategory _category = ItemCategory.Item;

        /// <summary>
        /// Item to give in case the category is item
        /// </summary>
        public GameObject _itemPrefab;

        /// <summary>
        /// Amount applies when the category is health (or bullets)
        /// </summary>
        public float _amount;

        void OnCollisionEnter2D(Collision2D collision)
        {
            // if the player collides with the exit, it will trigger a message to the in game state behaviour
            if (collision.gameObject.tag == GameTags.Player)
            {
                switch( _category )
                {
                    case ItemCategory.Item:
                        collision.gameObject.SendMessage(MessageNames.OnPickupItem, Instantiate(_itemPrefab), SendMessageOptions.RequireReceiver);
                        break;

                    case ItemCategory.Health:
                        collision.gameObject.SendMessage(MessageNames.OnPickupHealth, _amount, SendMessageOptions.RequireReceiver);
                        break;

                    default:
                        break;
                }
                Destroy(gameObject);
            }
        }
    }
}
