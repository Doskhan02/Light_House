using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class GameManager : MonoBehaviour
{
    [SerializeField] private Canvas worldSpaceCanvas;
    [SerializeField] private LightController lh_Light;
    [SerializeField] private GamePlayWindow gamePlayWindow;
    [SerializeField] private VictoryWindow victoryWindow;

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
    }

    public void StartGame()
    {
        isGameActive = true;
        lh_Light.Initialize();
        gamePlayWindow.Initialize();
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
            sessionTimeInSeconds = sessionTimeInSeconds + 1;
            sessionTime = 0;
            if (sessionTimeInSeconds == 60)
            {
                sessionTimeInMinutes = sessionTimeInMinutes + 1;
                sessionTimeInSeconds = 0;
            }
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
            CharacterSpawnSystem.Instance.SpawnCharacter(CharacterType.Ally, "Boat");
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
        victoryWindow.gameObject.SetActive(true);
        victoryWindow.Initialize(ScoreSystem.Score,sessionTimeInSeconds,sessionTimeInMinutes, true);
        isGameActive = false;
        CharacterSpawnSystem.Instance.CharacterWipe();
        for(int i = 0; i < returnedShips.Count; i++)
        {
            Destroy(returnedShips[i]);
        }
    }

    public void GameOver()
    {
        victoryWindow.gameObject.SetActive(true);
        victoryWindow.Initialize(ScoreSystem.Score, sessionTimeInSeconds, sessionTimeInMinutes, false);
        isGameActive = false;
        CharacterSpawnSystem.Instance.CharacterWipe();
        for (int i = 0; i < returnedShips.Count; i++)
        {
            Destroy(returnedShips[i]);
        }
    }

    public void Restart()
    {
        timeBetweenEnemySpawn = GameData.timeBetweenEnemySpawn - difficultyMultiplier * 0.1f;
        timeBetweenShipSpawn = GameData.timeBetweenShipSpawn;
        
        victoryWindow.gameObject.SetActive(false);
        StartGame();

    }
    public void GameContinue()
    {
        levelManager.NextLevel();
        timeBetweenEnemySpawn = GameData.timeBetweenEnemySpawn - difficultyMultiplier * 0.1f;
        timeBetweenShipSpawn = GameData.timeBetweenShipSpawn;

        victoryWindow.gameObject.SetActive(false);
        StartGame();
    }
}
