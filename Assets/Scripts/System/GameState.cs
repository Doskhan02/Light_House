using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameState
{
    None = 0,
    MainMenu = 1,
    GameSessionStart = 2,
    GameSessionPause = 3,
    GameSessionUnpause = 4,
    GameSessionEnd = 5,
    GameSessionRestart = 6,
    GameSessionNextLevel = 7
}
