/*
 * TDS (c) 2018 by Another Pointless Pun
 * TDS is licensed under a Creative Commons Attribution-ShareAlike 4.0 International License.
 * You should have received a copy of the license along with this work.  If not, see <http://creativecommons.org/licenses/by-sa/4.0/>.
 */
namespace Tds.GameScripts
{
    using UnityEngine;

    /// <summary>
    /// Behaviour which manages spawning item drops when characters die. This should be tied 
    /// to the level
    /// </summary>
    public class ItemDropBehaviour : MonoBehaviour
    {
        /// <summary>
        /// Prefabs of common items which can be dropped
        /// </summary>
        public GameObject[] _commonItemPrefabs;

        /// <summary>
        /// Chance an item will drop
        /// </summary>
        public float _commonChance = 0;

        /// <summary>
        /// Increment in the chance if a common item did not drop
        /// </summary>
        public float _commonIncrement = 0.1f;

        /// <summary>
        /// Value which prevents items dropping too quickly in succession
        /// </summary>
        public float _minimumCommonThreshold = 0.2f;
    
        public void OnCharacterDied(GameObject deadCharacter)
        {
            if (Random.value + _minimumCommonThreshold < _commonChance)
            {
                var instance = Instantiate(_commonItemPrefabs[Random.Range(0, _commonItemPrefabs.Length)]);
                instance.transform.position = deadCharacter.transform.position;
                _commonChance = 0;
            }
            else
            {
                _commonChance += _commonIncrement;
            }
        }

    }
}