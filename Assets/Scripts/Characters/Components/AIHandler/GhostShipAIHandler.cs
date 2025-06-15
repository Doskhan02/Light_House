using System.Collections;
using UnityEngine;

public class GhostShipAIHandler : IAIComponent
{
    private Character character;
    private float timeBetweenAttacks;
    private float timeBetweenBurn;
    private float timeBetweenHeal;
    private readonly Vector3 lightHit = GameManager.Instance.LightController.hit.point;
    private readonly UpgradeManager upgradeManager = GameManager.Instance.UpgradeManager;
    
    private GameObject ghostShipTarget;


    public void Initialize(Character selfCharacter)
    {
        this.character = selfCharacter;
        ghostShipTarget = GameObject.FindGameObjectWithTag("ShipTarget");
    }

    public void AIAction(Character target, AIState currentState, CharacterTypeData data) 
    {
        if(data is BasicEnemyData _data)
        {
            switch (currentState)
            {
                case AIState.None:
                    break;

                case AIState.Idle:

                    if (timeBetweenHeal <= 0 && character.lifeComponent.Health < character.lifeComponent.MaxHealth)
                    {
                        character.lifeComponent.Heal(_data.healAmount);
                        timeBetweenHeal = _data.healTime;
                    }
                    if (timeBetweenHeal > 0)
                        timeBetweenHeal -= Time.deltaTime;

                    AIAction(target, AIState.MoveToTarget, data);
                    break;

                case AIState.Fear:
                    if (character.isActiveAndEnabled == false)
                        return;
                    if (Vector3.Distance(lightHit, character.transform.position) > upgradeManager.Radius && timeBetweenBurn <= 0)
                    {
                        character.lifeComponent.SetDamage(upgradeManager.Damage);
                        character.StartCoroutine(GetHitEffect());
                        timeBetweenBurn = upgradeManager.AttackRate;
                    }
                    else if (timeBetweenBurn > 0)
                        timeBetweenBurn -= Time.deltaTime;

                    AIAction(target, AIState.MoveToTarget, data);
                    break;

                case AIState.MoveToTarget:
                    Vector3 directionToTarget = ghostShipTarget.transform.position - character.transform.position;
                    directionToTarget.Normalize();
                    character.movementComponent.Move(directionToTarget);
                    character.movementComponent.Rotate(directionToTarget);
                    break;
            }
        }
        
        if (Vector3.Distance(character.transform.position, ghostShipTarget.transform.position) < 12)
        {
            ScoreSystem.Instance.AddScore(-2);
            character.lifeComponent.SetDamage(character.lifeComponent.MaxHealth);
        }
    }

    private IEnumerator GetHitEffect()
    {
        character.CharacterData.CharacterModel.layer = LayerMask.NameToLayer("EnemyHit");
        yield return new WaitForSeconds(0.1f);
        character.CharacterData.CharacterModel.layer = LayerMask.NameToLayer("EnemyGhost");
    } 
}
