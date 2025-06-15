using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossEnemyAIHandler : IAIComponent
{
    private float elapsedTime = 0;
    
    public void AIAction(Character target, AIState currentState, CharacterTypeData data)
    {
        switch (currentState)
        {
            case AIState.Attack:
                
                break;
        }
    }

    public void Initialize(Character selfCharacter)
    {
        throw new System.NotImplementedException();
    }

    private void Hit(Transform target)
    {
        GameObject.Instantiate(target);
    }
    
}
