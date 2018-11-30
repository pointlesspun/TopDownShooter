/*
 * TDS (c) 2018 by Another Pointless Pun
 * TDS is licensed under a Creative Commons Attribution-ShareAlike 4.0 International License.
 * You should have received a copy of the license along with this work.  If not, see <http://creativecommons.org/licenses/by-sa/4.0/>.
 */
namespace Tds.GameScripts
{
    using UnityEngine;

    public class ItemSpawnBehaviour : MonoBehaviour
    {
        public GameObject[] _commonItemPrefabs;

        public float _commonChance = 0;

        public float _commonIncrement = 0.25f;

    
        public void OnCharacterDied(GameObject deadCharacter)
        {
            if (Random.value < _commonChance)
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