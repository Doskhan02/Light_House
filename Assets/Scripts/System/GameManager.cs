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

    public ShipSpawnSystem ShipSpawnSystem => shipSpawnSystem;
    public EnemySpawnSystem EnemySpawnSystem => enemySpawnSystem;
    public GameObject LightHouse => lightHouse;
    public LightController LightController => lh_Light;

    private bool isGameActive;
    private float timeBetweenShipSpawn = 3;
    private float timeBetweenEnemySpawn = 5;

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
    }

    public void StartGame()
    {
        isGameActive = true;
        lh_Light.Initialize();
    }

    private void Update()
    {
        if(!isGameActive)
            return;

        timeBetweenShipSpawn -= Time.deltaTime;
        timeBetweenEnemySpawn -= Time.deltaTime;

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
    }

    private void GameVictory()
    {
        Debug.Log("You Won!");
        isGameActive = false;
    }

    public void GameOver()
    {
        Debug.Log("You Lost:(");
        isGameActive = false;
    }
    private void SpawnShips()
    {
        Character ship = ShipSpawnSystem.GetCharacter(CharacterType.Ally);
        ship.transform.position = new Vector3(0 + GetOffset(), 0.5f, 40 + GetOffset());
        
        
        float GetOffset()
        {
            bool isPlus = Random.Range(0, 100) % 2 == 0;
            float offset = Random.Range(0, 10);
            return (isPlus) ? offset : (-1 * offset);
        }
     

    }
    private void SpawnEnemies()
    {
        Character enemy = EnemySpawnSystem.GetCharacter(CharacterType.Enemy);
        enemy.transform.position = new Vector3(50, 0.5f, Random.Range(-50,50));
                
    }
}
