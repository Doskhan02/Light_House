using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class GameManager : MonoBehaviour
{
    [SerializeField] private GameObject lightHouse;
    [SerializeField] private Canvas worldSpaceCanvas;
    [SerializeField] private LightController lh_Light;
    [SerializeField] private GamePlayWindow gamePlayWindow;
    [SerializeField] private VictoryWindow victoryWindow;

    #region Systems
    [SerializeField] private ShipSpawnSystem shipSpawnSystem;
    [SerializeField] private EnemySpawnSystem enemySpawnSystem;
    [SerializeField] private InputManager inputManager;
    [SerializeField] private ScoreSystem scoreSystem;
    [SerializeField] private UpgradeManager upgradeManager;
    [SerializeField] private LevelManager levelManager;
    [SerializeField] private GameData GameData;
    
    public ShipSpawnSystem ShipSpawnSystem => shipSpawnSystem;
    public EnemySpawnSystem EnemySpawnSystem => enemySpawnSystem;
    public InputManager InputManager => inputManager;
    public GameData gameData => GameData;
    public GameObject LightHouse => lightHouse;
    public Canvas WorldSpaceCanvas => worldSpaceCanvas;
    public LightController LightController => lh_Light;
    public ScoreSystem ScoreSystem => scoreSystem;
    public UpgradeManager UpgradeManager => upgradeManager;
    public LevelManager LevelManager => levelManager;
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
            ScoreSystem.EndGame();
        }

        if (timeBetweenShipSpawn < 0)
            {
            SpawnShips();
            timeBetweenShipSpawn = GameData.timeBetweenShipSpawn;
        }

        if (timeBetweenEnemySpawn < 0)
        {
            if (EnemySpawnSystem.ActiveCharacters.Count < GameData.maxEnemyCount)
            {
                SpawnEnemies();
                timeBetweenEnemySpawn = GameData.timeBetweenEnemySpawn - difficultyMultiplier * 0.1f;
            }
        }
    }

    private void GameVictory()
    {
        victoryWindow.gameObject.SetActive(true);
        victoryWindow.Initialize(ScoreSystem.Score,sessionTimeInSeconds,sessionTimeInMinutes, true);
        isGameActive = false;
        StartCoroutine(DisableAllCharacters());
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
        StartCoroutine(DisableAllCharacters());
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

    private void SpawnShips()
    {
        Character ship = shipSpawnSystem.GetCharacter(CharacterType.Ally);
        ship.transform.position = new Vector3(Random.Range(-30,30), 0.5f, 100);
        ship.gameObject.SetActive(true);
        ship.Initialize();
        ship.lifeComponent.Initialize(ship);
        ship.lifeComponent.OnCharacterDeath += CharacterDeathHandler;
    }
    private void SpawnEnemies()
    {
        Character enemy = EnemySpawnSystem.GetCharacter(CharacterType.Enemy);
        enemy.transform.position = new Vector3(Random.Range(-30, 30), -0.4f, Random.Range(40,50));
        enemy.gameObject.SetActive(true);
        enemy.Initialize();
        enemy.aiComponent.Initialize(enemy);
        enemy.lifeComponent.Initialize(enemy);
        enemy.lifeComponent.OnCharacterDeath += CharacterDeathHandler;

    }
    private void CharacterDeathHandler(Character deathCharacter)
    {
        deathCharacter.StopAllCoroutines();
        deathCharacter.lifeComponent.OnCharacterDeath -= CharacterDeathHandler;

        switch (deathCharacter.CharacterData.CharacterType)
        {
            case CharacterType.Ally:
                deathCharacter.gameObject.SetActive(false);
                shipSpawnSystem.ReturnCharacter(deathCharacter);
                break;

            case CharacterType.Enemy:
                deathCharacter.gameObject.SetActive(false);
                enemySpawnSystem.ReturnCharacter(deathCharacter);
                break;
        }
    }
    private IEnumerator DisableAllCharacters()
    {
        List<Character> shipList = new List<Character>(ShipSpawnSystem.ActiveCharacters);
        List<Character> enemyList = new List<Character>(EnemySpawnSystem.ActiveCharacters);

        foreach (Character ship in shipList)
        {
            CharacterDeathHandler(ship);
            yield return null;
        }

        foreach (Character enemy in enemyList)
        {
            CharacterDeathHandler(enemy);
            yield return null;
        }

        Debug.Log("All characters disabled.");
        Time.timeScale = 0;
    }
}
