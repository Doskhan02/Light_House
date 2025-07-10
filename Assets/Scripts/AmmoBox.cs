using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmmoBox : AllyCharacter
{
    [SerializeField] private GameObject[] piecePrefab;

    public override void Initialize()
    {
        base.Initialize();
        int index = Random.Range(0, piecePrefab.Length - 1);
        Debug.Log(index);
        var go = Instantiate(piecePrefab[Random.Range(0, piecePrefab.Length - 1)], transform.position, Quaternion.identity);
        go.transform.parent = transform;
    }

    public override void Update()
    {
        foreach (Collider collider in Physics.OverlapSphere(transform.position, 5f))
        {
            var ally = collider.gameObject.GetComponentInParent<AllyBossCharacter>();
            if (ally != null)
            {
                ally.AddPiece();
                lifeComponent.SetDamage(lifeComponent.MaxHealth);
            }
        }
    }
}
