using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GameData")]
public class GameData : ScriptableObject
{
    [Header("Spawn Time")]
    public float timeBetweenShipSpawn;
    public float timeBetweenEnemySpawn;
    
    [Header("Session Time & Target Score")]
    public float targetScore;
    public float sessionMaxTimeInMinutes;
    public float sessionMaxTimeInSeconds;
    
    [Header("Enemy Spawn Amounts")]
    public int wormSpawnAmount;
    public int deepGuardianSpawnAmount;
    public int deepGuardianSlimeSpawnAmount;
    public int ghostShipSpawnAmount;
    
    [Header("Boss Spawn Amounts")]
    public int bossAmount;
    public int bossAllyAmount;
    
    [Header("Ally Spawn Amounts")]
    public int basicShipAmount;
    public int bigShipAmount;
    public int ammoBoxAmount;
}
