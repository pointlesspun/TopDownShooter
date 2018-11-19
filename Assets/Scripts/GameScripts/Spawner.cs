/*
 * TDS (c) 2018 by Another Pointless Pun
 * TDS is licensed under a Creative Commons Attribution-ShareAlike 4.0 International License.
 * You should have received a copy of the license along with this work.  If not, see <http://creativecommons.org/licenses/by-sa/4.0/>.
 */
namespace Tds.GameScripts
{
    using System.Collections.Generic;
    using UnityEngine;

    using Tds.GameStateScripts;

    /// <summary>
    /// Game object which will spawn other game objects (monsters most often).
    /// </summary>
    public class Spawner : MonoBehaviour
    {
        /// <summary>
        /// Object spawned by this spawner
        /// </summary>
        public GameObject _prefab;

        /// <summary>
        /// Max number of alive spawned objects this element can have
        /// </summary>
        public int _maxSpawns = 4;

        /// <summary>
        /// Interval in seconds at which spanwer checks if it can spawn another game object
        /// </summary>
        public float _spawnInterval = 10.0f;
        
        private List<GameObject> _spawnedObjects = new List<GameObject>();
        private float _lastSpawnTime = 0;

        private void Start()
        {
            _lastSpawnTime = Time.time;
        }

        void Update()
        {
            // can the spawner try to spawn another object ?
            if (Time.time - _lastSpawnTime > _spawnInterval)
            {
                CleanUpSpawnList();
            
                // can we spawn another object ?
                if (_spawnedObjects.Count < _maxSpawns)
                {
                    var spawn = GameObject.Instantiate<GameObject>(_prefab);
                    _spawnedObjects.Add(spawn);
                    spawn.transform.position = transform.position;

                    if (InGameStateBehaviour._inGameStateObject != null)
                    {
                        spawn.transform.parent = InGameStateBehaviour._inGameStateObject.transform;
                    }
                }

                _lastSpawnTime = Time.time;
            }
        }

        private void CleanUpSpawnList()
        {
            for (int i = 0; i < _spawnedObjects.Count;)
            {
                if (_spawnedObjects[i] == null || !_spawnedObjects[i].activeInHierarchy)
                {
                    _spawnedObjects.RemoveAt(i);
                }
                else
                {
                    i++;
                }
            }
        }
    }
}
