using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LifeComponent : ILifeComponent
{
    private Character selfCharacter;
    private float currentHealth;

    public float MaxHealth { get; private set; } = 50f;

    public float Health
    {
        get => currentHealth;
        private set
        {
            currentHealth = value;
            if (currentHealth > MaxHealth)
                currentHealth = MaxHealth;
            if (currentHealth <= 0)
            {
                currentHealth = 0;
                SetDeath();
            }
        }
    }

    public event Action<Character> OnCharacterDeath;


    public LifeComponent(float maxHealth = 50f)
    {
        MaxHealth = maxHealth;
    }

    public void Initialize(Character selfCharacter)
    {
        this.selfCharacter = selfCharacter;
        Health = MaxHealth;
    }

    public void SetDamage(float damage)
    {
        Health -= damage;
        Debug.Log("Character " + selfCharacter.name + " took damage: " + damage + ". Current Health: " + Health);

    }

    private void SetDeath()
    {
        Debug.Log("Character " + selfCharacter.name + " has died.");
        OnCharacterDeath?.Invoke(selfCharacter);
    }
}
