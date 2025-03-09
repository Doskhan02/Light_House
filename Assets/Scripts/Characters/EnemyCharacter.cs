using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyCharacter : Character
{
    private RaycastHit hit;
    private Vector3 direction;
    private float timeBetweenAttacks;
    private float timeBetweenBurn;
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
                if(distanceToLight < 5)
                    continue;
                if (list[i].transform.position.z >= 70)
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
        timeBetweenAttacks = 1;
        timeBetweenBurn = 0.5f;
    }

    
    public override void Update()
    {
        timeBetweenBurn -= Time.deltaTime;
        hit = GameManager.Instance.LightController.hit;
        float distance = Vector3.Distance(hit.point, transform.position);
        
        if (distance < 5f)
        {
            if (timeBetweenBurn <= 0) 
            {
                lifeComponent.SetDamage(5);
                timeBetweenBurn = 0.5f;
            }
            aiComponent.AIAction(Target, AIState.Fear);
            
        }
        else
        {
            if (Target == null)
            {
                aiComponent.AIAction(Target,AIState.Idle);
            }
            else
            {
                if (Vector3.Distance(transform.position, Target.gameObject.transform.position) < 3f)
                    aiComponent.AIAction(Target,AIState.Attack);
                else
                aiComponent.AIAction(Target, AIState.MoveToTarget);

            }
        }       
    }
}
