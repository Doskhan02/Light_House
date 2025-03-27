using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStateManager : MonoBehaviour
{
    public static GameStateManager Instance { get; private set; }

    [SerializeField] private CharacterSpawnSystem spawnSystem;
    [SerializeField] private TimeManager timeManager;
    [SerializeField] private LightController lightController;

    private int sessionTimeInSeconds;
    private int sessionTimeInMinutes;
    private GameState currentGameState = GameState.MainMenu;
    private bool isGameSessionActive;

    public GameState CurrentGameState => currentGameState;

    public event Action<GameState> OnGameStateChanged;
    public event Action OnGameSessionStarted;
    public event Action OnGameSessionEnded;

    public void Awake()
    {
        Instance = this;
    }
    public void GameStart()
    {
        timeManager.ResetTime();
    }
    void Update()
    {
        if (isGameSessionActive)
        {
            timeManager.UpdateSessionTime();

        }
    }

    public void ChangeGameState(GameState newGameState)
    {
        currentGameState = newGameState;
        switch(currentGameState)
        {
            case GameState.MainMenu:

                break;
            case GameState.GameSessionStart:
                timeManager.ResetTime();
                OnGameSessionStarted?.Invoke();
                break;
            case GameState.GameSessionPause:
                PauseGameSession(false);
                break;
            case GameState.GameSessionUnpause:
                PauseGameSession(true);
                break;
            case GameState.GameSessionEnd:
                OnGameSessionStarted = null;
                break;
            case GameState.GameSessionRestart:

                break;
            case GameState.GameSessionNextLevel:

                break;
        }
    }

    public void StartGameSession()
    {

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
