using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStateManager : MonoBehaviour
{
    public static GameStateManager Instance { get; private set; }

    [SerializeField] private CharacterSpawnSystem spawnSystem;
    [SerializeField] private GameData gameData;
    [SerializeField] private LightController lightController;

    private int sessionTimeInSeconds;
    private int sessionTimeInMinutes;
    private float sessionTime;
    private GameState currentGameState = GameState.MainMenu;
    private bool isGameSessionActive;

    public GameState CurrentGameState => currentGameState;

    public void Awake()
    {
        Instance = this;
    }
    public void GameStart()
    {
        sessionTime = 0;
        sessionTimeInSeconds = 0;
        sessionTimeInMinutes = 0;
    }
    void Update()
    {
        if (isGameSessionActive)
        {
            sessionTime += Time.deltaTime;
            if (sessionTime > 1)
            {
                sessionTimeInSeconds++;
                sessionTime = 0;
                if (sessionTimeInSeconds % 60 == 0)
                {
                    sessionTimeInMinutes++;
                    sessionTimeInSeconds = 0;
                }
            }
            if (sessionTimeInMinutes >= gameData.sessionMaxTimeInMinutes && 
                sessionTimeInSeconds >= gameData.sessionMaxTimeInSeconds)
            {
                EndGameSession(false);
            }
        }
        else
        {
            sessionTime = 0;
            sessionTimeInSeconds = 0;
            sessionTimeInMinutes = 0;
        }
    }

    public void ChangeGameState(GameState newGameState)
    {
        currentGameState = newGameState;
        switch(currentGameState)
        {
            case GameState.MainMenu:
                ReturnToMainMenu();
                break;
            case GameState.GameSessionStart:
                break;
            case GameState.GameSessionPause:
                PauseGameSession(false);
                break;
            case GameState.GameSessionUnpause:
                PauseGameSession(true);
                break;
            case GameState.GameSessionRestart:

                break;
            case GameState.GameSessionNextLevel:

                break;
        }
    }

    public void StartGameSession()
    {
        isGameSessionActive = true;
    }
    public void PauseGameSession(bool IsCurrentlyPaused)
    {
        if (IsCurrentlyPaused)
        {
            Time.timeScale = 1;
        }
        else
        {
            Time.timeScale = 0;
        }
    }
    public void EndGameSession(bool IsVictory)
    {

    }
    public void RestartGameSession()
    {

    }
    public void ReturnToMainMenu()
    {

    }
    public void NextLevel()
    {

    }
}
