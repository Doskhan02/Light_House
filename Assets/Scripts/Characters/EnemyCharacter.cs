using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyCharacter : Character
{
    private RaycastHit hit;
    private Vector3 direction;
    private Coroutine patrolRoutine;
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
                if(distanceToLight < 3)
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
        base.Initialize();
        movementComponent.Move(transform.position);
        patrolRoutine = StartCoroutine(Patrol());
    }

    
    public override void Update()
    {
        hit = GameManager.Instance.LightController.hit;
        float distance = Vector3.Distance(hit.point, transform.position);

        if (distance < 7f)
        {
            //lifeComponent.SetDamage(5);
            direction = transform.position - hit.point;
            movementComponent.Rotate(direction);
            movementComponent.Move(direction);
            
        }
        else
        {
            if (Target == null)
            {
                if(patrolRoutine == null)
                {
                    patrolRoutine = StartCoroutine(Patrol());
                }
            }
            else
            {
                StopPatrolling();
                if (Vector3.Distance(transform.position, Target.gameObject.transform.position) < 2.25f)
                {
                    Target.lifeComponent.SetDamage(50);
                }
                Vector3 rotationDirection = Target.transform.position - transform.position;
                movementComponent.Rotate(rotationDirection);
                movementComponent.Move(rotationDirection);
            }
        }       
    }
    private void StopPatrolling()
    {
        if (patrolRoutine != null)
        {
            StopCoroutine(patrolRoutine);
            patrolRoutine = null;
        }
    }
    private IEnumerator Patrol()
    {
        while (true)
        {
            float GetOffset()
            {
                bool isPlus = Random.Range(0, 100) % 2 == 0;
                float offset = Random.Range(10, 30);
                return (isPlus) ? offset : (-1 * offset);
            }
            Vector3 target = new Vector3(GetOffset(), 0, GetOffset());
            direction = target - transform.position;

            movementComponent.Rotate(direction);

            float patrolTime = Random.Range(2f, 4f);
            float elapsedTime = 0f;

            while (elapsedTime < patrolTime)
            {
                movementComponent.Move(direction);
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            yield return new WaitForSeconds(0.2f);
        }
    }
}
