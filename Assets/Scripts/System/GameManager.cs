using System;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

public class GameManager : MonoBehaviour
{
    [SerializeField] private Canvas worldSpaceCanvas;
    [SerializeField] private LightController lh_Light;
    [SerializeField] private GameObject volume;
    [SerializeField] private int pieceAmount;
    [SerializeField] private GameObject piecePrefab;

    [SerializeField] private GameObject[] sailedShips;

    #region Systems
    [SerializeField] private InputManager inputManager;
    [SerializeField] private ScoreSystem scoreSystem;
    [SerializeField] private UpgradeManager upgradeManager;
    [SerializeField] private LevelManager levelManager;
    [SerializeField] private GameData[] datas;
    [SerializeField] private WindowService windowService; 
    
    public InputManager InputManager => inputManager;
    public GameData gameData => _gameData;
    public Canvas WorldSpaceCanvas => worldSpaceCanvas;
    public LightController LightController => lh_Light;
    public ScoreSystem ScoreSystem => scoreSystem;
    public UpgradeManager UpgradeManager => upgradeManager;
    public LevelManager LevelManager => levelManager;
    public WindowService WindowService => windowService;
    #endregion

    private GameData _gameData;
    private bool isGameActive;
    private float timeBetweenShipSpawn;
    private float timeBetweenEnemySpawn;
    private float sessionTime;
    private int sessionTimeInSeconds;
    private int sessionTimeInMinutes;
    private float difficultyMultiplier;

    private bool isBossFight;
    
    public int SessionTimeInMinutes => sessionTimeInMinutes;
    public int SessionTimeInSeconds => sessionTimeInSeconds;

    public List<GameObject> returnedShips;
    
    public int PieceAmount => pieceAmount;

    public event Action<int, int> OnSessionTimeUpdated;

    public bool IsGameActive {  get { return isGameActive; } }

    public bool IsCutsceenActive;

    public event Action OnCutsceen;

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
        if (levelManager.CurrentLevel <= datas.Length)
        {
            _gameData = datas[levelManager.CurrentLevel - 1];
        }
        else
        {
            _gameData = datas[datas.Length - 1];
        }
        windowService.Initialize();
        foreach (GameObject ship in returnedShips)
        {
            ship.SetActive(false);
        }
        
    }

    public void StartGame()
    {
        levelManager.ChangeLevel(6);
        ResetCutsceneState();
        ResetSailedShips();
        if(levelManager.CurrentLevel % 6 == 0)
        {
            isGameActive = false;
            isBossFight = true;
            SceenSetUp();
            Cutsceen();
        }
        else
        {
            isBossFight = false;
        }
        if (isGameActive)
            return;
        isGameActive = true;
        if(levelManager.CurrentLevel <= datas.Length)
        {
            _gameData = datas[levelManager.CurrentLevel - 1];
        }
        else
        {
            _gameData = datas[^2];
        }
        lh_Light.Initialize();
        scoreSystem.StartGame();
        OnCurrentScoreChanged(0);
        scoreSystem.OnScoreUpdated += OnCurrentScoreChanged;
        difficultyMultiplier = LevelManager.GetDifficultyMultiplier();
        sessionTime = 0;
        sessionTimeInSeconds = (int)_gameData.sessionMaxTimeInSeconds;
        sessionTimeInMinutes = (int)_gameData.sessionMaxTimeInMinutes;
        CharacterSpawnSystem.Instance.Initialize();
        timeBetweenShipSpawn = _gameData.timeBetweenShipSpawn;
        timeBetweenEnemySpawn = _gameData.timeBetweenEnemySpawn;
        Time.timeScale = 1;
    }

    private void OnCurrentScoreChanged(int score)
    {
        // Calculate progress as a float between 0 and 1
        float progress = (float)score / _gameData.targetScore;
    
        // Clamp progress to ensure it doesn't exceed 1.0
        progress = Mathf.Clamp01(progress);
    
        // Calculate how many ships should be visible (one per 10% progress)
        int shipsToShow = Mathf.FloorToInt(progress * 5);
    
        // Ensure we don't exceed the available ships array length
        shipsToShow = Mathf.Min(shipsToShow, sailedShips.Length);
    
        // Show/hide ships based on progress
        for (int i = 0; i < sailedShips.Length; i++)
        {
            if (i < shipsToShow)
            {
                sailedShips[i].SetActive(true);
            }
            else
            {
                sailedShips[i].SetActive(false);
            }
        }
    }

    public void ResetSailedShips()
    {
        foreach (GameObject ship in returnedShips)
        {
            ship.SetActive(false);
        }
    }

    private void Update()
    {
        if(!isGameActive)
            return;

        if (IsCutsceenActive)
        {
            return;
        }

        if (!isBossFight)
        {
            Timer();
        }
        timeBetweenShipSpawn -= Time.deltaTime;
        timeBetweenEnemySpawn -= Time.deltaTime;
        
        if (sessionTimeInMinutes == 0 && sessionTimeInSeconds == 0)
        {
            if (scoreSystem.Score < _gameData.targetScore)
            {
                GameOver();
            }
        }
        

        if (scoreSystem.Score >= _gameData.targetScore)
        {
            GameVictory();
        }

        if (timeBetweenShipSpawn < 0)
        {
            if (LevelManager.CurrentLevel % 6 == 0)
            {
                float GetRandom(float min, float max)
                {
                    float randomValue = UnityEngine.Random.Range(min, max);
                    int sign = UnityEngine.Random.value < 0.5f ? -1 : 1;
                    return randomValue * sign;
                }
                CharacterSpawnSystem.Instance.SpawnCharacter(CharacterType.Ally, "AmmoBox", 
                    new Vector3(GetRandom(-10,10), 0, UnityEngine.Random.Range(40,90)));
            }
            else
            {
                CharacterSpawnSystem.Instance.SpawnCharacter(CharacterType.Ally);
            }
            timeBetweenShipSpawn = _gameData.timeBetweenShipSpawn;
        }

        if (timeBetweenEnemySpawn < 0)
        {
            CharacterSpawnSystem.Instance.SpawnCharacter(CharacterType.Enemy);
            timeBetweenEnemySpawn = _gameData.timeBetweenEnemySpawn;
        }
    }

    private void SpawnBoss()
    {
        if (LevelManager.CurrentLevel % 6 == 0)
        {
            CharacterSpawnSystem.Instance.SpawnCharacter(CharacterType.Enemy, "DT(BOSS)", new Vector3(0, 0, 130));
        }
    }

    private void SceenSetUp()
    {
        CharacterSpawnSystem.Instance.SpawnCharacter(CharacterType.Ally, "Boat3", new Vector3(-70, 0, 40));
        Invoke(nameof(SpawnBoss), 13f);
        CharacterSpawnSystem.Instance.SpawnCharacter(CharacterType.Ally, "AmmoBox", new Vector3(0, 0, 40));
    }

    private void Timer()
    {
        sessionTime += Time.deltaTime;
        if (sessionTime > 1)
        {
            sessionTime = 0;
            sessionTimeInSeconds--;

            if (sessionTimeInSeconds < 0)
            {
                if (sessionTimeInMinutes > 0)
                {
                    sessionTimeInMinutes--;
                    sessionTimeInSeconds = 59;
                }
                else
                {
                    sessionTimeInSeconds = 0;
                }
            }

            OnSessionTimeUpdated?.Invoke(sessionTimeInSeconds, sessionTimeInMinutes);
        }
    }

    public void GameVictory()
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
        LevelManager.Instance.CheckPointReset();
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
        ResetCutsceneState();
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
        ResetCutsceneState();
        timeBetweenEnemySpawn = _gameData.timeBetweenEnemySpawn;
        timeBetweenShipSpawn = _gameData.timeBetweenShipSpawn;
        StartGame();

    }
    public void GameContinue()
    {
        ResetCutsceneState();
        timeBetweenEnemySpawn = _gameData.timeBetweenEnemySpawn;
        timeBetweenShipSpawn = _gameData.timeBetweenShipSpawn;
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

    public void Cutsceen()
    {
        IsCutsceenActive = true;
        OnCutsceen.Invoke();
        windowService.HideAllWindows(true);
        windowService.ShowWindow<CutsceenWindow>(false);
    }
    private void ResetCutsceneState()
    {
        IsCutsceenActive = false;
        isBossFight = false;
    }

    public void HardReset()
    {
        DataPersistanceManager.Instance.ResetGame();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
    Application.Quit();
#endif
    }
}