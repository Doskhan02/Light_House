using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyCharacter : Character
{
    private RaycastHit hit;
    private Vector3 direction;
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
    void Start()
    {
        base.Initialize();
        movementComponent.Move(transform.position);
    }

    
    public override void Update()
    {
        hit = GameManager.Instance.LightController.hit;
        float distance = Vector3.Distance(hit.point, transform.position);

        if (distance < 7f)
        {
            direction = transform.position - hit.point;
            movementComponent.Rotate(direction);
            movementComponent.Move(direction);
            
        }
        else
        {
            if (Target == null)
            {
                transform.position = this.transform.position;
            }
            else
            {
                if (Vector3.Distance(transform.position, Target.gameObject.transform.position) < 2)
                {
                    Target.gameObject.GetComponent<AllyCharacter>().Sinked();
                }
                Vector3 rotationDirection = Target.transform.position - transform.position;
                movementComponent.Rotate(rotationDirection);
                movementComponent.Move(rotationDirection);
            }
        }       
    }
}
