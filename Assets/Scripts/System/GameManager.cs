using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class GameManager : MonoBehaviour
{
    [SerializeField] private Canvas worldSpaceCanvas;
    [SerializeField] private LightController lh_Light;

    #region Systems
    [SerializeField] private InputManager inputManager;
    [SerializeField] private ScoreSystem scoreSystem;
    [SerializeField] private UpgradeManager upgradeManager;
    [SerializeField] private LevelManager levelManager;
    [SerializeField] private GameData GameData;
    [SerializeField] private WindowService windowService; 
    
    public InputManager InputManager => inputManager;
    public GameData gameData => GameData;
    public Canvas WorldSpaceCanvas => worldSpaceCanvas;
    public LightController LightController => lh_Light;
    public ScoreSystem ScoreSystem => scoreSystem;
    public UpgradeManager UpgradeManager => upgradeManager;
    public LevelManager LevelManager => levelManager;
    public WindowService WindowService => windowService;
    #endregion

    private bool isGameActive;
    private float timeBetweenShipSpawn;
    private float timeBetweenEnemySpawn;
    private float sessionTime;
    private int sessionTimeInSeconds;
    private int sessionTimeInMinutes;
    private float difficultyMultiplier;

    public int SessionTimeInMinutes => sessionTimeInMinutes;
    public int SessionTimeInSeconds => sessionTimeInSeconds;

    public List<GameObject> returnedShips;

    public event Action<int, int> OnSessionTimeUpdated;

    public bool IsGameActive {  get { return isGameActive; } }
    public static GameManager Instance { get; private set; }

    private void Awake()
    {
        Application.targetFrameRate = 60;
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            Initialize();
        }
        else
        {
            Destroy(this.gameObject);
        }
    }
    private void Initialize()
    {
        isGameActive = false;
        scoreSystem = new ScoreSystem();
        windowService.Initialize();
    }

    public void StartGame()
    {
        if (isGameActive)
            return;
        isGameActive = true;
        lh_Light.Initialize();
        scoreSystem.StartGame();
        sessionTime = 0;
        sessionTimeInSeconds = 0;
        sessionTimeInMinutes = 0;
        difficultyMultiplier = LevelManager.GetDifficultyMultiplier();
        timeBetweenShipSpawn = GameData.timeBetweenShipSpawn;
        timeBetweenEnemySpawn = GameData.timeBetweenEnemySpawn - difficultyMultiplier * 0.1f;
        Time.timeScale = 1;
        
    }

    private void Update()
    {
        if(!isGameActive)
            return;

        timeBetweenShipSpawn -= Time.deltaTime;
        timeBetweenEnemySpawn -= Time.deltaTime;



        sessionTime += Time.deltaTime;
        if (sessionTime > 1)
        {
            sessionTimeInSeconds ++;
            sessionTime = 0;
            if (sessionTimeInSeconds == 60)
            {
                sessionTimeInMinutes = sessionTimeInMinutes + 1;
                sessionTimeInSeconds = 0;
            }
            OnSessionTimeUpdated?.Invoke(sessionTimeInSeconds, sessionTimeInMinutes);
        }

        if (sessionTimeInMinutes == GameData.sessionMaxTimeInMinutes && 
            sessionTimeInSeconds == GameData.sessionMaxTimeInSeconds)
        {
            if (scoreSystem.Score < GameData.targetScore)
            {
                GameOver();
            }
        }
        if (scoreSystem.Score >= GameData.targetScore)
        {
            GameVictory();
        }

        if (timeBetweenShipSpawn < 0)
        {
            CharacterSpawnSystem.Instance.SpawnCharacter(CharacterType.Ally);
            timeBetweenShipSpawn = GameData.timeBetweenShipSpawn;
        }

        if (timeBetweenEnemySpawn < 0)
        {
            CharacterSpawnSystem.Instance.SpawnCharacter(CharacterType.Enemy);
            timeBetweenEnemySpawn = GameData.timeBetweenEnemySpawn - difficultyMultiplier * 0.1f;
        }
    }

    private void GameVictory()
    {
        isGameActive = false;
        windowService.HideAllWindows(true);
        windowService.ShowWindow<GameVictoryWindow>(false);
        ScoreSystem.Instance.CalculateReward();
        LevelManager.Instance.NextLevel();
        CharacterSpawnSystem.Instance.CharacterWipe();
        for(int i = 0; i < returnedShips.Count; i++)
        {
            Destroy(returnedShips[i]);
        }
        returnedShips.Clear();
        Time.timeScale = 0;
    }

    public void GameOver()
    {
        isGameActive = false;
        windowService.HideAllWindows(true);
        windowService.ShowWindow<DefeatWindow>(false);  
        CharacterSpawnSystem.Instance.CharacterWipe();
        for (int i = 0; i < returnedShips.Count; i++)
        {
            Destroy(returnedShips[i]);
        }
        returnedShips.Clear();
        Time.timeScale = 0;
    }
    public void ReturnToMainMenu()
    {
        isGameActive = false;
        windowService.HideAllWindows(true);
        windowService.ShowWindow<MainMenuWindow>(false);
        CharacterSpawnSystem.Instance.CharacterWipe();
        for (int i = 0; i < returnedShips.Count; i++)
        {
            Destroy(returnedShips[i]);
        }
        returnedShips.Clear();
        Time.timeScale = 1;
    }

    public void Restart()
    {
        timeBetweenEnemySpawn = GameData.timeBetweenEnemySpawn - difficultyMultiplier * 0.1f;
        timeBetweenShipSpawn = GameData.timeBetweenShipSpawn;
        StartGame();

    }
    public void GameContinue()
    {
        timeBetweenEnemySpawn = GameData.timeBetweenEnemySpawn - difficultyMultiplier * 0.1f;
        timeBetweenShipSpawn = GameData.timeBetweenShipSpawn;
        StartGame();
    }
    public void GamePause()
    {
        Time.timeScale = 0;
    }
    public void GameResume()
    {
        Time.timeScale = 1;
    }
}
