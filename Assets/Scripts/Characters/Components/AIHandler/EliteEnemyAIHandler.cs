using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EliteEnemyAIHandler : IAIComponent
{
    private Character character;
    private float timeBetweenAttacks;
    private float timeBetweenBurn;
    private float timeBetweenHeal;
    private Vector3 lightHit = GameManager.Instance.LightController.hit.point;
    private UpgradeManager upgradeManager = GameManager.Instance.UpgradeManager;
    private LevelManager levelManager = GameManager.Instance.LevelManager;

    private Vector3 patrolTarget;
    private float patrolTime;
    private float elapsedPatrolTime;


    public void Initialize(Character selfCharacter)
    {
        this.character = selfCharacter;
    }

    public void AIAction(Character target, AIState currentState, BasicEnemyData data)
    {
        switch (currentState)
        {
            case AIState.None:
                break;

            case AIState.Idle:
                character.StopAllCoroutines();
                Patrol();

                if (timeBetweenHeal <= 0)
                {
                    character.lifeComponent.Heal(data.healAmount);
                    timeBetweenHeal = data.healTime;
                }
                if (timeBetweenHeal > 0)
                    timeBetweenHeal -= Time.deltaTime;

                break;

            case AIState.Fear:
                if (character.isActiveAndEnabled == false)
                    return;
                if (Vector3.Distance(lightHit, character.transform.position) > upgradeManager.Radius && timeBetweenBurn <= 0)
                {
                    character.lifeComponent.SetDamage(upgradeManager.Damage);
                    timeBetweenBurn = upgradeManager.AttackRate;
                }
                if (timeBetweenBurn > 0)
                    timeBetweenBurn -= Time.deltaTime;
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
                    target.lifeComponent.SetDamage(data.damage * levelManager.GetDifficultyMultiplier());
                    ParticleManager.Instance.PlayHitParticleEffect(target.transform);
                    timeBetweenAttacks = data.timeBetweenAttacks;
                }
                if (timeBetweenAttacks > 0)
                    timeBetweenAttacks -= Time.deltaTime;

                break;
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
        patrolTarget = new Vector3(GetOffset(), -0.4f, Random.Range(10, 30));
        if (patrolTarget == character.transform.position)
        {
            SetNewPatrolTarget();
        }
        patrolTime = Random.Range(2f, 4f);
        elapsedPatrolTime = 0f;
    }
}
