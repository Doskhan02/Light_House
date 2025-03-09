using System.Collections;
using UnityEngine;

public class BasicEnemyAIHandler : IAIComponent
{
    private Character character;
    private float timeBetweenAttacks;
    private float timeBetweenBurn;
    private float distanceToTarget;
    private Vector3 lightHit = GameManager.Instance.LightController.hit.point;
    private Coroutine patrolRoutine;

    private Vector3 patrolTarget;
    private float patrolTime;
    private float elapsedPatrolTime;

    public void Initialize(Character selfCharacter)
    {
        this.character = selfCharacter;
    }

    public void AIAction(Character target, AIState currentState) 
    {
        switch (currentState) 
        {
            case AIState.None:
                break;

            case AIState.Idle:
                Patrol();
                break;

            case AIState.Fear:

                if (timeBetweenBurn <= 0)
                {
                    character.lifeComponent.SetDamage(5);
                    timeBetweenBurn = 0.5f;
                }
                if (timeBetweenBurn > 0)
                    timeBetweenBurn -= Time.deltaTime;

                Vector3 direction = character.transform.position - lightHit;
                direction.Normalize();
                character.movementComponent.Move(direction);
                character.movementComponent.Rotate(direction);
                break;

            case AIState.MoveToTarget:
                Vector3 directionToTarget = target.transform.position - character.transform.position;
                directionToTarget.Normalize();
                character.movementComponent.Move(directionToTarget);
                character.movementComponent.Rotate(directionToTarget);
                break;

            case AIState.Attack:
                
                if (timeBetweenAttacks <= 0)
                {
                    target.lifeComponent.SetDamage(20);
                    timeBetweenAttacks = 1;
                }
                if (timeBetweenAttacks > 0)
                    timeBetweenAttacks -= Time.deltaTime;
                
                break;
        }
    }
    private void Patrol()
    {
        elapsedPatrolTime += Time.deltaTime;
        if (elapsedPatrolTime >= patrolTime)
        {
            SetNewPatrolTarget();
        }

        Vector3 direction = patrolTarget - character.transform.position;
        direction.y = 0;
        direction.Normalize();
        character.movementComponent.Move(direction);
        character.movementComponent.Rotate(direction);
    }

    private void SetNewPatrolTarget()
    {
        float GetOffset() => (Random.Range(0, 2) == 0 ? 1 : -1) * Random.Range(10, 30);
        patrolTarget = new Vector3(GetOffset(), 0, Random.Range(10, 30));
        patrolTime = Random.Range(2f, 4f);
        elapsedPatrolTime = 0f;
    }
}
