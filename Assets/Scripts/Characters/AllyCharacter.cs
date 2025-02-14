using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AllyCharacter : Character
{
    [SerializeField] private GameObject target;
    private Vector3 direction;

    public void Start()
    {
        if (target == null)
        {
            target = GameManager.Instance.LightHouse;
        }
        base.Initialize();
        movementComponent.Rotate(direction);
    }
    public override void Update()
    {
        direction = target.transform.position - transform.position;
        movementComponent.Move(direction);
        movementComponent.Rotate(direction);
        if (direction.magnitude < 8)
        {
            gameObject.SetActive(false);
            GameManager.Instance.ShipSpawnSystem.ReturnCharacter(this);
        }
    }
}
