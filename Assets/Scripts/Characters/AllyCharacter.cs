using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AllyCharacter : Character
{
    [SerializeField] private GameObject target;
    private Vector3 direction;
    private Character selfCharacter;

    public void Start()
    {
        if (target == null)
        {
            target = GameManager.Instance.LightHouse;
        }
        base.Initialize();
        movementComponent.Rotate(direction);
        selfCharacter = this.gameObject.GetComponent<Character>();
    }
    public override void Update()
    {
        direction = target.transform.position - transform.position;
        movementComponent.Move(direction);
        movementComponent.Rotate(direction);
        if (direction.magnitude < 9)
        {
            
            GameManager.Instance.ShipSpawnSystem.ReturnCharacter(selfCharacter);
            gameObject.SetActive(false);

        }
    }
}
