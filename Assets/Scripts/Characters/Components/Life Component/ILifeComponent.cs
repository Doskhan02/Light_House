using System;
using UnityEngine;

public interface ILifeComponent : ICharacterComponent
{
    public event Action<Character> OnCharacterDeath;
    public event Action<Character> OnCharacterHealthChange;

    float MaxHealth { get; }
    float Health { get; }
    public void SetDamage(float damage);
    public void Heal(float healPoints);
}
