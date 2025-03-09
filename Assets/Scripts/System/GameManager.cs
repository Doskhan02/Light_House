using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class GameManager : MonoBehaviour
{
    [SerializeField] private ShipSpawnSystem shipSpawnSystem;
    [SerializeField] private EnemySpawnSystem enemySpawnSystem;
    [SerializeField] private InputManager inputManager;

    [SerializeField] private GameObject lightHouse;
    [SerializeField] private LightController lh_Light;
    [SerializeField] private VolumetricLightController VLight;
    [SerializeField] private ScoreSystem scoreSystem;
    [SerializeField] private GamePlayWindow gamePlayWindow;
    [SerializeField] private VictoryWindow victoryWindow;

    [SerializeField] private GameData GameData;

    public ShipSpawnSystem ShipSpawnSystem => shipSpawnSystem;
    public EnemySpawnSystem EnemySpawnSystem => enemySpawnSystem;
    public InputManager InputManager => inputManager;
    public GameData gameData => GameData;
    public GameObject LightHouse => lightHouse;
    public LightController LightController => lh_Light;
    public ScoreSystem ScoreSystem => scoreSystem;

    private bool isGameActive;
    private float timeBetweenShipSpawn;
    private float timeBetweenEnemySpawn;
    private float sessionTime;
    private int sessionTimeInSeconds;
    private int sessionTimeInMinutes;

    public int SessionTimeInMinutes => sessionTimeInMinutes;
    public int SessionTimeInSeconds => sessionTimeInSeconds;

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
        VLight.Initialize();
        gamePlayWindow.Initialize();
        scoreSystem.StartGame();
        sessionTime = 0;
        sessionTimeInSeconds = 0;
        sessionTimeInMinutes = 0;
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
            if (EnemySpawnSystem.ActiveCharacters.Count < 10)
            {
                SpawnEnemies();
                timeBetweenEnemySpawn = GameData.timeBetweenEnemySpawn;
            }
        }
    }

    private void GameVictory()
    {
        victoryWindow.gameObject.SetActive(true);
        victoryWindow.Initialize(ScoreSystem.Score,sessionTimeInSeconds,sessionTimeInMinutes);
        isGameActive = false;
        Time.timeScale = 0;
    }

    public void GameOver()
    {
        Debug.Log("You Lost:(");
        isGameActive = false;
        Time.timeScale = 0;
    }

    public void Restart()
    {
        timeBetweenEnemySpawn = GameData.timeBetweenEnemySpawn;
        timeBetweenShipSpawn = GameData.timeBetweenShipSpawn;

        List<Character> shipList = shipSpawnSystem.ActiveCharacters;
        for (int i = 0; i <= shipList.Count; i++)
        {
            shipList[i].lifeComponent.SetDamage(shipList[i].lifeComponent.MaxHealth);         
        }

        List<Character> enemyList = enemySpawnSystem.ActiveCharacters;
        for (int i = 0; i <= enemyList.Count; i++)
        {
            enemyList[i].lifeComponent.SetDamage(enemyList[i].lifeComponent.MaxHealth);
        }

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
        enemy.transform.position = new Vector3(Random.Range(-30, 30), 0.5f, Random.Range(40,50));
        enemy.gameObject.SetActive(true);
        enemy.Initialize();
        enemy.aiComponent.Initialize(enemy);
        enemy.lifeComponent.Initialize(enemy);
        enemy.lifeComponent.OnCharacterDeath += CharacterDeathHandler;

    }
    private void CharacterDeathHandler(Character deathCharacter)
    {
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
        
        deathCharacter.lifeComponent.OnCharacterDeath -= CharacterDeathHandler;
    }
}
