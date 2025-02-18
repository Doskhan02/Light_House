using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class GameManager : MonoBehaviour
{
    [SerializeField] ShipSpawnSystem shipSpawnSystem;
    [SerializeField] EnemySpawnSystem enemySpawnSystem;
    [SerializeField] private GameObject lightHouse;
    [SerializeField] private LightController lh_Light;
    [SerializeField] private VolumetricLightController VLight;
    [SerializeField] private ScoreSystem scoreSystem;
    [SerializeField] private GamePlayWindow gamePlayWindow;

    public ShipSpawnSystem ShipSpawnSystem => shipSpawnSystem;
    public EnemySpawnSystem EnemySpawnSystem => enemySpawnSystem;
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
        Debug.Log("You Won!");
        isGameActive = false;
        Time.timeScale = 0;
    }

    public void GameOver()
    {
        Debug.Log("You Lost:(");
        isGameActive = false;
    }
    private void SpawnShips()
    {
        Character ship = ShipSpawnSystem.GetCharacter(CharacterType.Ally);
        ship.transform.position = new Vector3(Random.Range(-30,30), 0.5f, 80);
       
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
                
    }
}
