using UnityEngine;

[CreateAssetMenu(fileName = "BasicEnemyData", menuName = "Scriptable Objects/Character Type Data/Basic Enemy Data")]
public class BasicEnemyData : CharacterTypeData
{
    public float timeBetweenAttacks;
    public float damage;
    public float attackDistance;
    public float fearDuration;

    public float healTime;
    public float healAmount;
}
