using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GameData")]
public class GameData : ScriptableObject
{
    public int sessionTimeInSeconds;
    public int sessionTimeInMinutes;

    public int timeBetweenShipSpawn;
    public int timeBetweenEnemySpawn;

    public int targetScore;
}
