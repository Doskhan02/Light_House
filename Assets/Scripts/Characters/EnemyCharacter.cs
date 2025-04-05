using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyCharacter : Character
{
    [SerializeField] private BasicEnemyData data;
    [SerializeField] private EnemyType enemyType;

    private UpgradeManager upgradeManager;
    private RaycastHit hit;
    private bool isCoroutineRunning = false;

    public override Character Target 
    { 
        get 
        {
            Character target = null;
            float minDistance = float.MaxValue;
            List<Character> list = CharacterSpawnSystem.Instance.CharacterFactory.GetActiveCharacters(CharacterType.Ally);
            for (int i = 0; i < list.Count; i++)
            {
                if (!list[i].gameObject.activeSelf)
                    continue;
                if(enemyType == EnemyType.Basic)
                {
                    float distanceToLight = Vector3.Distance(list[i].transform.position, hit.point);
                    if (distanceToLight < upgradeManager.Radius)
                        continue;
                }
                if (list[i].transform.position.z >= 60)
                    continue;
                float distanceBetween = Vector3.Distance(list[i].transform.position, transform.position);
                if (distanceBetween < minDistance)
                {
                    target = list[i];
                    minDistance = distanceBetween;
                }
            }
            return target; 
        } 
    }
    public override void Initialize()
    {
        lifeComponent = new LifeComponent();
        if(enemyType == EnemyType.Basic)
        {
            aiComponent = new BasicEnemyAIHandler();
        }
        else if (enemyType == EnemyType.Elite)
        {
            aiComponent = new EliteEnemyAIHandler();
        }
        effectComponent = new EffectComponent();
        base.Initialize();
        movementComponent.Move(transform.position);
        isCoroutineRunning = false;
        upgradeManager = GameManager.Instance.UpgradeManager;
    }


    public override void Update()
    {
        if (effectComponent != null)
        {
            effectComponent.CheckForEffectsInLight();

            ((EffectComponent)effectComponent).EffectUpdate(Time.deltaTime);
        }
        if (isCoroutineRunning)
            return;

        hit = GameManager.Instance.LightController.hit;

        float distance = Vector3.Distance(hit.point, transform.position);
        
        if (distance < upgradeManager.Radius && enemyType == EnemyType.Basic)
        {
            StartCoroutine(Fear());
        }
        else if (distance < upgradeManager.Radius && enemyType == EnemyType.Elite)
        {
             aiComponent.AIAction(Target, AIState.Fear, data);
        }
        else
        {
            if (Target == null)
            {
                aiComponent.AIAction(Target, AIState.Idle, data);
            }
            else
            {
                if (Vector3.Distance(transform.position, Target.gameObject.transform.position) < data.attackDistance)
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
        isCoroutineRunning=false;
    }
}
