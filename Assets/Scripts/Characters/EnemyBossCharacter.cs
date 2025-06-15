using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBossCharacter : EnemyCharacter
{
    [SerializeField] private GameObject tentacleHit;
    [SerializeField] private GlobalTentacle tentacle;
    [SerializeField] private float coolDown;
    [SerializeField] private int enemyAmount;
    [SerializeField] private float spawnCoolDown;
    private float elapsedTime = 0f;
    private float hitTimer = 0f;
    private float spawnTimer = 0f;
    public override Character Target
    {
        get
        {
            Character target = null;
            float minDistance = float.MaxValue; // Start with max value to find closest
            List<Character> list = CharacterSpawnSystem.Instance.CharacterFactory.GetActiveCharacters(CharacterType.Ally);

            foreach (var character in list)
            {
                if (!character.gameObject.activeSelf)
                    continue;

                if (character == this)
                    continue;

                float distanceBetween = Vector3.Distance(character.transform.position, transform.position);

                if (distanceBetween < minDistance)
                {
                    minDistance = distanceBetween;
                    target = character;
                }
            }

            return target;
        }
    }

    public override void Initialize()
    {
        base.Initialize();
        elapsedTime = coolDown;
        hitTimer = data.timeBetweenAttacks;
        spawnTimer = spawnCoolDown;
    }

    public override void Update()
    {
        if (Target != null)
        {
            if (elapsedTime < 0)
            {
                tentacle.Initialize();
                elapsedTime = coolDown;
            }
            elapsedTime -= Time.deltaTime;
            if (hitTimer < 0)
            {
                Vector3 offset = new Vector3(Random.Range(-2,2), 0, Random.Range(-2,2));
                GameObject.Instantiate(tentacleHit, Target.transform.position + offset, Quaternion.identity);
                hitTimer = data.timeBetweenAttacks;
            }
            hitTimer -= Time.deltaTime;

            if (spawnTimer <= 0)
            {
                for (int i = 0; i < enemyAmount; i++)
                {
                    CharacterSpawnSystem.Instance.SpawnCharacter(CharacterType.Enemy, "Worm",transform.position);
                }
                spawnTimer = spawnCoolDown;
            }
            spawnTimer -= Time.deltaTime;
        }
    }
}
