using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class InGameStateBehaviour : MonoBehaviour, IGameState
{
    public static GameObject _inGameStateObject;

    private GameStateType _state = GameStateType.NotStarted;

    public GameObject titleScreenGameState;
    public GameObject levelPrefab;

    public GameStateType State
    {
        get
        {
            return _state;
        }
    }

    private GameObject _level;
    private GameObject _player;

    public void StartGameState()
    {
        if (!gameObject.activeInHierarchy)
        {
            gameObject.SetActive(true);
        }

        _state = GameStateType.Started;
        _level = Instantiate<GameObject>(levelPrefab);
        _level.transform.parent = transform;
        _player = GameObject.FindGameObjectWithTag("Player");
    }

    public void StopGameState()
    {
        _state = GameStateType.Stopped;
        Destroy(_level);
        titleScreenGameState.GetComponent<IGameState>().StartGameState();
        gameObject.SetActive(false);
    }

    public void Start()
    {
        _inGameStateObject = gameObject;
    }

    public void Update()
    {
        if ( _state == GameStateType.Started)
        {
            if (_player == null || !_player.activeInHierarchy)
            {
                StopGameState();
            }
        }
    }
}

