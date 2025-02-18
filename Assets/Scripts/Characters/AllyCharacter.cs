using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AllyCharacter : Character
{
    [SerializeField] private GameObject target;
    private Vector3 direction;
    private Character selfCharacter;
    private int score;

    public void Start()
    {
        if (target == null)
        {
            target = GameManager.Instance.LightHouse;
        }
        base.Initialize();
        movementComponent.Rotate(direction);
        selfCharacter = this.gameObject.GetComponent<Character>();
        score = UnityEngine.Random.Range(2, 4);
    }
    public override void Update()
    {
        float distance = Vector3.Distance(GameManager.Instance.LightController.hit.point, transform.position);
        if (distance < 2)
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
            GameManager.Instance.ScoreSystem.AddScore(score);
            GameManager.Instance.ShipSpawnSystem.ReturnCharacter(selfCharacter);
            gameObject.SetActive(false);

        }
    }

    public void Sinked()
    {
        GameManager.Instance.ShipSpawnSystem.ReturnCharacter(selfCharacter);
        gameObject.SetActive(false);
    }
}
