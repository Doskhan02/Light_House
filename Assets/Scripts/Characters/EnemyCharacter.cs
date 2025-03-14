using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class EnemyCharacter : Character
{
    [SerializeField] private BasicEnemyData data;

    private LightData LightData;
    private RaycastHit hit;
    private Vector3 direction;
    private float timeBetweenBurn;
    private float timeBetweenHeal;
    private bool isCoroutineRunning = false;

    public override Character Target 
    { 
        get 
        {
            Character target = null;
            float minDistance = float.MaxValue;
            List<Character> list = GameManager.Instance.ShipSpawnSystem.ActiveCharacters;
            for (int i = 0; i < list.Count; i++)
            {
                if (!list[i].gameObject.activeSelf)
                    continue;
                float distanceToLight = Vector3.Distance(list[i].transform.position, hit.point);
                if(distanceToLight < LightData.baseRadius)
                    continue;
                if (list[i].transform.position.z >= 60)
                    continue;
                if (list[i].CharacterData.CharacterType == CharacterType.Enemy)
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
        aiComponent = new BasicEnemyAIHandler();
        base.Initialize();
        movementComponent.Move(transform.position);
        LightData = GameManager.Instance.LightController.LightData;
    }


    public override void Update()
    {
        if(isCoroutineRunning)
            return;
        hit = GameManager.Instance.LightController.hit;

        float distance = Vector3.Distance(hit.point, transform.position);
        
        if (distance < LightData.baseRadius + 2)
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
            aiComponent.AIAction(Target, AIState.Fear, data);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        isCoroutineRunning=false;
    }
}
