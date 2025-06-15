using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class EnemyCharacter : Character
{
    [SerializeField] public BasicEnemyData data;
    [SerializeField] private EnemyType enemyType;
    [SerializeField] private Canvas canvas;

    private UpgradeManager upgradeManager;
    private RaycastHit hit;
    private bool isCoroutineRunning = false;
    private float timeBetweenBurn = 0;

    public override Character Target 
    { 
        get 
        {
            Character target = null;
            float minDistance = data.detectionRange;
            List<Character> list = CharacterSpawnSystem.Instance.CharacterFactory.GetActiveCharacters(CharacterType.Ally);
            for (int i = 0; i < list.Count; i++)
            {
                if (!list[i].gameObject)
                    continue;
                if(enemyType == EnemyType.Basic)
                {
                    float distanceToLight = Vector3.Distance(list[i].transform.position, hit.point);
                    if (distanceToLight < upgradeManager.Radius)
                        continue;
                }
                /*if (list[i].transform.position.z >= 80)
                    continue;*/
                float distanceBetween = Vector3.Distance(list[i].transform.position, transform.position);
                if (distanceBetween < minDistance)
                {
                    minDistance = distanceBetween;
                    target = list[i];
                }
            }
            return target; 
        } 
    }
    public override void Initialize()
    {
        canvas = GameManager.Instance.WorldSpaceCanvas;
        lifeComponent = new LifeComponent();
        if(enemyType != EnemyType.GhostShip)
        {
            aiComponent = new BasicEnemyAIHandler();
        }
        else if (enemyType == EnemyType.GhostShip)
        {
            aiComponent = new GhostShipAIHandler();
        }
        effectComponent = new EffectComponent();
        base.Initialize();
        movementComponent.Move(transform.position);
        isCoroutineRunning = false;
        upgradeManager = GameManager.Instance.UpgradeManager;
        CharacterData.Healthbar.Initialize();
        if(data.isMinionParent)
        {
            lifeComponent.OnCharacterDeath += SpawnMinions;
        }
        SetUpHealthbar();
        
    }


    public override void Update()
    {
        if (effectComponent != null)
        {
            effectComponent.CheckForEffectsInLight();

            ((EffectComponent)effectComponent).EffectUpdate(Time.deltaTime);
        }

        hit = GameManager.Instance.LightController.hit;
        if (isActiveAndEnabled == false)
            return;

        if (Vector3.Distance(hit.point, transform.position) < upgradeManager.Radius && timeBetweenBurn <= 0)
        {
            lifeComponent.SetDamage(upgradeManager.Damage);
            timeBetweenBurn = upgradeManager.AttackRate;
        }
        else if (timeBetweenBurn > 0)
            timeBetweenBurn -= Time.deltaTime;

        if (isCoroutineRunning)
            return;

        float distance = Vector3.Distance(hit.point, transform.position);
        
        if (distance < upgradeManager.Radius && enemyType == EnemyType.Basic)
        {
            StartCoroutine(Fear());
        }
        else
        {
            if (Target == null)
            {
                aiComponent.AIAction(Target, AIState.Idle, data);
            }
            else
            {
                if (Vector3.Distance(transform.position, Target.transform.position) < data.attackDistance)
                    aiComponent.AIAction(Target, AIState.Attack, data);
                else
                    aiComponent.AIAction(Target, AIState.MoveToTarget, data);
            }
        }
    }
    private IEnumerator Fear()
    {
        isCoroutineRunning = true;
        float elapsedTime = 0;
        while (elapsedTime < data.fearDuration)
        {
            if(gameObject.activeSelf == false)
                break;
            aiComponent.AIAction(Target, AIState.Fear, data);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        isCoroutineRunning =false;
    }

    public void SetUpHealthbar()
    {
        CharacterData.Healthbar.transform.SetParent(canvas.transform);
    }
    public void SpawnMinions(Character character)
    {
        float offset = 1f; // Make sure this is a float
        if (data.isMinionParent)
        {
            for (int i = 0; i < data.maxMinions; i++)
            {
                // Alternate direction: +offset for even, -offset for odd
                float direction = (i % 2 == 0) ? 1f : -1f;

                Vector3 spawnPosition = character.transform.position + new Vector3(offset * direction, 0, 0);

                CharacterSpawnSystem.Instance.SpawnCharacter(
                    CharacterType.EnemyMinion,
                    "DGSlime(minion)",
                    spawnPosition
                );
            }
        }
    }
    private void OnDisable()
    {
        StopAllCoroutines();
        if (lifeComponent != null)
        {
            lifeComponent.OnCharacterDeath -= SpawnMinions;
        }
    }
}
