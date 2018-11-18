using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleScreenStateBehaviour : MonoBehaviour, IGameState {

    private GameStateType _state = GameStateType.NotStarted;

    public GameObject inGameState;
    public GameObject titleScreenUI;

    public bool isFireButtonUp = false;

    public GameStateType State
    {
        get
        {
            return _state;
        }
    }

    public void StopGameState()
    {
        _state = GameStateType.Stopped;
        titleScreenUI.SetActive(false);
        inGameState.GetComponent<IGameState>().StartGameState();
        gameObject.SetActive(false);
    }

    public void StartGameState()
    {
        if (!gameObject.activeInHierarchy)
        {
            gameObject.SetActive(true);
        }

        isFireButtonUp = false;
        _state = GameStateType.Started;
        titleScreenUI.SetActive(true);
    }
    
    void Start()
    {
        StartGameState();
    }

    void Update ()
    {
        if (!isFireButtonUp)
        {
            isFireButtonUp = !Input.GetButton("Fire1");
        }
        
        if (isFireButtonUp)
        {
            if (_state == GameStateType.Started && Input.GetButton("Fire1"))
            {
                StopGameState();
            }
        }
	}
}
