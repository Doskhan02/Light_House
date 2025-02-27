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

    public ShipSpawnSystem ShipSpawnSystem => shipSpawnSystem;
    public EnemySpawnSystem EnemySpawnSystem => enemySpawnSystem;
    public InputManager InputManager => inputManager;

    public GameObject LightHouse => lightHouse;
    public LightController LightController => lh_Light;
    public ScoreSystem ScoreSystem => scoreSystem;

    private bool isGameActive;
    private float timeBetweenShipSpawn = 3;
    private float timeBetweenEnemySpawn = 5;
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
        timeBetweenShipSpawn = 3;
        timeBetweenEnemySpawn = 5;
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

            if (timeBetweenShipSpawn < 0)
        {
            SpawnShips();
            timeBetweenShipSpawn = 3;
        }

        if (timeBetweenEnemySpawn < 0)
        {
            if (EnemySpawnSystem.ActiveCharacters.Count < 3)
            {
                SpawnEnemies();
                timeBetweenEnemySpawn = 5;
            }
        }

        if (scoreSystem.Score == 10 || scoreSystem.Score > 10) 
        {
            GameVictory();
            ScoreSystem.EndGame();
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
    }

    public void Restart()
    {
        timeBetweenEnemySpawn = 5;
        timeBetweenShipSpawn = 3;

        List<Character> shipList = shipSpawnSystem.ActiveCharacters;
        for (int i = 0; i <= shipList.Count; i++)
        {
            shipList[i].gameObject.SetActive(false);
            shipSpawnSystem.ReturnCharacter(shipList[i]);
            
        }

        List<Character> enemyList = enemySpawnSystem.ActiveCharacters;
        for (int i = 0; i <= enemyList.Count; i++)
        {
            enemyList[i].gameObject.SetActive(false);
            enemySpawnSystem.ReturnCharacter(enemyList[i]);
            
        }

        victoryWindow.gameObject.SetActive(false);
        StartGame();

    }

    private void SpawnShips()
    {
        Character ship = shipSpawnSystem.GetCharacter(CharacterType.Ally);
        ship.transform.position = new Vector3(Random.Range(-30,30), 0.5f, 80);
        ship.gameObject.SetActive(true);
        ship.Initialize();
        ship.lifeComponent.Initialize(ship);
        ship.lifeComponent.OnCharacterDeath += CharacterDeathHandler;



        /*float GetOffset()
        {
            bool isPlus = Random.Range(0, 100) % 2 == 0;
            float offset = Random.Range(0, 30);
            return (isPlus) ? offset : (-1 * offset);
        }*/
     

    }
    private void SpawnEnemies()
    {
        Character enemy = EnemySpawnSystem.GetCharacter(CharacterType.Enemy);
        enemy.transform.position = new Vector3(Random.Range(-30, 30), 0.5f, Random.Range(40,50));
        enemy.gameObject.SetActive(true);
        enemy.Initialize();
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
