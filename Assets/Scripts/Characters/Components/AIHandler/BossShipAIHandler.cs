using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossShipAIHandler : IAIComponent
{
    private Character character;

    private RaycastHit hit;

    public void Initialize(Character selfCharacter)
    {
        character = selfCharacter;
        
    }

    public void AIAction(Character target, AIState currentState, CharacterTypeData data)
    {
        if(data is BasicAllyData _data)
        {
            switch (currentState)
            {
                case AIState.MoveToTarget:
                    var direction = target.transform.position - character.transform.position;
                    character.movementComponent.Rotate(direction);
                    if (target is EnemyCharacter && Vector3.Distance(target.transform.position, character.transform.position) < 20)
                        return;
                    character.movementComponent.Move(direction);
                    break;
                case AIState.Attack:
                    break;
                case AIState.Idle:
                    hit = GameManager.Instance.LightController.hit;
                    var directionTolight = hit.point - character.transform.position;
                    character.movementComponent.Move(directionTolight);
                    character.movementComponent.Rotate(directionTolight);
                    break;
            }
        }
    }
}
