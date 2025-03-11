using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GameData")]
public class GameData : ScriptableObject
{
    public float sessionTimeInSeconds;
    public float sessionTimeInMinutes;

    public float timeBetweenShipSpawn;
    public float timeBetweenEnemySpawn;

    public float targetScore;
    public float sessionMaxTimeInMinutes;
    public float sessionMaxTimeInSeconds;
    public int maxEnemyCount;

}
