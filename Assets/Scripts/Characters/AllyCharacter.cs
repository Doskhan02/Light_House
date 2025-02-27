using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AllyCharacter : Character
{
    [SerializeField] private GameObject target;
    private Vector3 direction;
    private int score;


    public override void Initialize()
    {
        base.Initialize();
        lifeComponent = new LifeComponent();
        if (target == null)
        {
            target = GameManager.Instance.LightHouse;
        }
        
        movementComponent.Rotate(direction);
        score = UnityEngine.Random.Range(2, 4);
    }
    public override void Update()
    {
        float distance = Vector3.Distance(GameManager.Instance.LightController.hit.point, transform.position);
        if (distance < 4)
        {
            movementComponent.Speed = CharacterData.DefaultSpeed + 2;
        }
        else
        {
            movementComponent.Speed = CharacterData.DefaultSpeed;
        }

        direction = target.transform.position - transform.position;
        movementComponent.Move(direction);
        movementComponent.Rotate(direction);
        if (direction.magnitude < 9)
        {
            lifeComponent.SetDamage(lifeComponent.MaxHealth);
            GameManager.Instance.ScoreSystem.AddScore(score);
            gameObject.SetActive(false);

        }
    }

    

}
