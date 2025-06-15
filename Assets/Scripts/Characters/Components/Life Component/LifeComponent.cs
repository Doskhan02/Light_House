using System;
using System.Collections;
using UnityEngine;

public class LifeComponent : ILifeComponent
{
    private Character selfCharacter;
    private float currentHealth;
    private int originalLayer;
    private Coroutine resetLayerCoroutine;

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
    public event Action<Character> OnCharacterHealthChange;

    public LifeComponent(float maxHealth = 50f)
    {
        MaxHealth = maxHealth;
    }

    public void Initialize(Character selfCharacter)
    {
        this.selfCharacter = selfCharacter;
        MaxHealth = selfCharacter.CharacterData.CharacterTypeData.defaultMaxHP;
        Health = MaxHealth;
        switch (selfCharacter.CharacterType)
        {
            case CharacterType.Enemy:
                originalLayer = LayerMask.NameToLayer("Enemy");
                break;
            case CharacterType.FakeAlly:
                originalLayer = LayerMask.NameToLayer("EnemyGhost");
                break;
            case CharacterType.Ally:
                originalLayer = LayerMask.NameToLayer("Ally");
                break;
            case CharacterType.EnemyMinion:
                originalLayer = LayerMask.NameToLayer("Enemy");
                break;
            default:
                Debug.LogError($"Invalid character type: {selfCharacter.CharacterType}");
                break;
        }

        Debug.Log($"LifeComponent initialized for {selfCharacter.name} with layer {originalLayer}");
    }

    public void SetDamage(float damage)
    {
        Health -= damage;

        if (resetLayerCoroutine != null)
            selfCharacter.StopCoroutine(resetLayerCoroutine);

        if(selfCharacter.isActiveAndEnabled)
            resetLayerCoroutine = selfCharacter.StartCoroutine(ResetLayer());

        OnCharacterHealthChange?.Invoke(selfCharacter);
    }

    private void SetDeath()
    {
        OnCharacterDeath?.Invoke(selfCharacter);
    }

    public void Heal(float healPoints)
    {
        Health += healPoints;
        OnCharacterHealthChange?.Invoke(selfCharacter);
    }

    private IEnumerator ResetLayer()
    {
        // flash into the �EnemyHit� layer
        selfCharacter.CharacterData.CharacterModel.layer = LayerMask.NameToLayer("EnemyHit");

        yield return new WaitForSeconds(0.1f);

        // restore the stored original layer index
        selfCharacter.CharacterData.CharacterModel.layer = originalLayer;
    }
}
