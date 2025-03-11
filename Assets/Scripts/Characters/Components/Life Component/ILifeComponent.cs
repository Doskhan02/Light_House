using System;
using UnityEngine;

public interface ILifeComponent : ICharacterComponent
{
    public event Action<Character> OnCharacterDeath;


    float MaxHealth { get; }
    float Health { get; }
    public void SetDamage(float damage);
    public void Heal(float healPoints);
}
