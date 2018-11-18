using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour {

    public GameObject _prefab;

    public int _maxSpawns = 4;
    public float _spawnInterval = 10.0f;

    private List<GameObject> _spawnedObjects = new List<GameObject>();

    private float _lastSpawnTime = 0;

    private void Start()
    {
        _lastSpawnTime = Time.time;
    }

    void Update () {

		if ( Time.time - _lastSpawnTime > _spawnInterval )
        {
            for ( int i = 0; i < _spawnedObjects.Count; )
            {
                if ( _spawnedObjects[i] == null || !_spawnedObjects[i].activeInHierarchy)
                {
                    _spawnedObjects.RemoveAt(i);
                }
                else
                {
                    i++;
                }
            }

            if (_spawnedObjects.Count < _maxSpawns)
            {
                var spawn = GameObject.Instantiate<GameObject>(_prefab);
                _spawnedObjects.Add(spawn);
                spawn.transform.position = transform.position;
                spawn.transform.parent = transform;
            }

            _lastSpawnTime = Time.time;
        }
	} 
}
