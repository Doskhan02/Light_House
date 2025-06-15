using UnityEngine;

public class BasicShipAIHandler : IAIComponent
{
    private Character character;
    GameObject go;
    public void Initialize(Character selfCharacter)
    {
        character = selfCharacter;
        go = GameObject.FindGameObjectWithTag("ShipTarget");
    }

    public void AIAction(Character target, AIState currentState, CharacterTypeData data)
    {
        if(data is BasicAllyData _data)
        {
            switch (currentState)
            {
                case AIState.MoveToTarget:
                    
                    var direction = go.transform.position - character.transform.position;
                    character.movementComponent.Move(direction);
                    character.movementComponent.Rotate(direction);
                    if (direction.magnitude < 12)
                    {
                        character.lifeComponent.SetDamage(character.lifeComponent.MaxHealth);
                        ScoreSystem.Instance.AddScore(_data.Score);
                        character.gameObject.SetActive(false);
                    }
                    break;
            }
        }
        
    }
}
