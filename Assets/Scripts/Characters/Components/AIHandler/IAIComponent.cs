using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IAIComponent
{
    public void Initialize(Character selfCharacter);
    public void AIAction(Character target, AIState currentState, BasicEnemyData data);
}
