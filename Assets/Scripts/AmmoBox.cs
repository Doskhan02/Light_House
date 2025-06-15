using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmmoBox : AllyCharacter
{
    [SerializeField] private float ammoAmount;
    
    public override void Update()
    {
        foreach (Collider collider in Physics.OverlapSphere(transform.position, 5f))
        {
            var ally = collider.gameObject.GetComponentInParent<AllyBossCharacter>();
            if (ally != null)
            {
                ally.ReloadTurrets(ammoAmount);
                lifeComponent.SetDamage(lifeComponent.MaxHealth);
            }
        }
    }
}
