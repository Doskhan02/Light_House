using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnemyBossCharacter : EnemyCharacter
{
    [SerializeField] private GameObject tentacleHit;
    [SerializeField] private GameObject stun;
    [SerializeField] private GlobalTentacle tentacle;
    [SerializeField] private float coolDown;
    [SerializeField] private int enemyAmount;
    [SerializeField] private float spawnCoolDown;
    [SerializeField] private float stunCoolDown;
    [SerializeField] private Collider hitBox;
    
    private Vector3[] spawnPositions;

    private AllyBossCharacter Target => 
        CharacterSpawnSystem.Instance.CharacterFactory.GetActiveCharacters(CharacterType.Ally, "Boat3").FirstOrDefault() as AllyBossCharacter;

    private enum AttackState
    {
        None,
        TentacleAttack,
        SpawnEnemies,
        HitAttack,
        Stun,
    }

    private AttackState currentAttack = AttackState.None;
    private float attackTimer = 10f;

    public override void Initialize()
    {
        base.Initialize();
        ChooseNextAttack();
        spawnPositions = new Vector3[3];
        spawnPositions[0] = transform.position;
        spawnPositions[1] = new Vector3(-40, 0, 40);
        spawnPositions[2] = new Vector3(40, 0, 40);
    }

    public override void Update()
    {
        if (Target == null) return;

        if (attackTimer > 0)
        {
            attackTimer -= Time.deltaTime;
        }
        else
        {
            switch (currentAttack)
            {
                case AttackState.TentacleAttack:
                    CharacterData.Animator.SetTrigger("Dive");
                    hitBox.enabled = false;
                    Invoke(nameof(GlobalAttack), 2);
                    break;
                case AttackState.HitAttack:
                    Vector3 dir = Target.transform.forward;
                    GameObject.Instantiate(tentacleHit, Target.transform.position + dir * 5f, Quaternion.identity, transform);
                    break;
                case AttackState.SpawnEnemies:
                    CharacterData.Animator.SetTrigger("Spawn");
                    for (int i = 0; i < enemyAmount; i++)
                    {
                        CharacterSpawnSystem.Instance.SpawnCharacter(CharacterType.Enemy, "Worm(BOSS)", spawnPositions[Random.Range(0, spawnPositions.Length)]);
                    }
                    break;
                case AttackState.Stun:
                    CharacterData.Animator.SetTrigger("Dive");
                    hitBox.enabled = false;
                    Invoke(nameof(Stun), 2);
                    break;
            }
            
            ChooseNextAttack();
        }
    }

    private void GlobalAttack()
    {
        tentacle?.Initialize();
        CancelInvoke(nameof(Rise));
        Invoke(nameof(Rise),9.5f);
    }

    private void Stun()
    {
        if (stun == null)
        {
            Debug.LogError("Stun effect prefab is not assigned!");
            return;
        }

        if (Target == null)
        {
            Debug.LogError("Target is null!");
            return;
        }

        // Инстанциируем эффект ошеломления
        GameObject.Instantiate(stun, Target.transform.position, Quaternion.identity, transform);

        // Спауним 3 врага типа Worm вокруг цели
        for (int i = 0; i < 3; i++)
        {
            Vector3 spawnPosition = Target.transform.position + RandomOffset();
            CharacterSpawnSystem.Instance.SpawnCharacter(CharacterType.Enemy, "Worm(BOSS)", spawnPosition);
        }
    }

    private Vector3 RandomOffset()
    {
        // Генерируем случайное смещение по каждой оси независимо
        float x = Random.Range(7f, 12f) * (Random.value < 0.5f ? -1f : 1f);
        float z = Random.Range(7f, 12f) * (Random.value < 0.5f ? -1f : 1f);

        return new Vector3(x, 0, z);
    }

    public void Rise()
    {
        CancelInvoke(nameof(Rise));
        CharacterData.Animator.SetTrigger("Rise");
        hitBox.enabled = true;
    }

    private void ChooseNextAttack()
    {
        // Здесь можно добавить кастомные веса для разных атак, если нужно
        int random = Random.Range(0, 4); // три возможные атаки

        switch (random)
        {
            case 0:
                currentAttack = AttackState.TentacleAttack;
                attackTimer = coolDown;
                break;
            case 1:
                currentAttack = AttackState.HitAttack;
                attackTimer = data.timeBetweenAttacks;
                break;
            case 2:
                currentAttack = AttackState.SpawnEnemies;
                attackTimer = spawnCoolDown;
                break;
            case 3:
                currentAttack = AttackState.Stun;
                attackTimer = stunCoolDown;
                break;
        }
    }
}