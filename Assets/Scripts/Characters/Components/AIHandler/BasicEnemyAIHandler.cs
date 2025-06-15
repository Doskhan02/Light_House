using System.Collections;
using UnityEngine;

public class BasicEnemyAIHandler : IAIComponent
{
    private Character character;
    private float timeBetweenAttacks;
    private readonly Vector3 lightHit = GameManager.Instance.LightController.hit.point;
    private readonly LevelManager levelManager = LevelManager.Instance;

    private Vector3 patrolTarget;
    private float patrolTime;
    private float elapsedPatrolTime;


    public void Initialize(Character selfCharacter)
    {
        this.character = selfCharacter;
    }

    public void AIAction(Character target, AIState currentState, CharacterTypeData data) 
    {
        if(data is BasicEnemyData basicEnemyData)
        {
            switch (currentState)
            {
                case AIState.None:
                    break;

                case AIState.Idle:
                    Patrol();
                    break;

                case AIState.Fear:
                    if (character.isActiveAndEnabled == false)
                        return;
                    Vector3 direction = character.transform.position - lightHit;
                    direction.Normalize();
                    if (character.isActiveAndEnabled == false)
                        return;
                    character.movementComponent.Speed = character.CharacterData.CharacterTypeData.defaultSpeed + basicEnemyData.fearSpeedUpFactor;
                    character.movementComponent.Move(direction);
                    character.movementComponent.Rotate(direction);
                    character.movementComponent.Speed = character.CharacterData.CharacterTypeData.defaultSpeed;
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
                        target.lifeComponent.SetDamage(basicEnemyData.damage * levelManager.GetDifficultyMultiplier());
                        ParticleManager.Instance.PlayHitParticleEffect(target.transform);
                        if (character.CharacterData.Animator != null)
                        {
                            character.CharacterData.Animator.SetTrigger("Attack");
                        }
                        timeBetweenAttacks = basicEnemyData.timeBetweenAttacks;
                    }
                    if (timeBetweenAttacks > 0)
                        timeBetweenAttacks -= Time.deltaTime;

                    break;
            }
        }
        
    }
    private void Patrol()
    {
        elapsedPatrolTime += Time.deltaTime;
        if (Vector3.Distance(character.transform.position, patrolTarget) < 0.5f || elapsedPatrolTime >= patrolTime)
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
        patrolTarget = new Vector3(GetOffset(), -1f, Random.Range(10, 80));
        if(patrolTarget == character.transform.position)
        {
            SetNewPatrolTarget();
        }
        patrolTime = Random.Range(2f, 4f);
        elapsedPatrolTime = 0f;
    }
}
