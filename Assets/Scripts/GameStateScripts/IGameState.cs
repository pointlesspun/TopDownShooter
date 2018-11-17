using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public enum GameStateType
{
    NotStarted,
    Started,
    Stopped
}

public interface IGameState
{
    GameStateType State { get; }
    void StartGameState();
    void StopGameState();
}

