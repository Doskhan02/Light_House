using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectComponent : IEffectComponent
{
    private Character selfCharacter;
    private List<ActiveEffect> activeEffects = new List<ActiveEffect>();
    private Dictionary<Type, List<ActiveEffect>> effectsByType = new();

    // Visual feedback
    private Material originalMaterial;
    private Color originalColor;

    public void Initialize(Character selfCharacter)
    {
        this.selfCharacter = selfCharacter;
    }

    public void EffectUpdate(float deltaTime)
    {
        // Copy the list to avoid modification during iteration
        List<ActiveEffect> effectsCopy = new List<ActiveEffect>(activeEffects);

        foreach (ActiveEffect effect in effectsCopy)
        {
            // Update duration
            if (effect.effect.duration > 0)
            {
                effect.remainingDuration -= deltaTime;

                // Remove effect if duration expired
                if (effect.remainingDuration <= 0)
                {
                    RemoveEffect(effect.effect);
                    continue;
                }
            }
            if (effect.effect is DOTEffect dotEffect)
            {
                // Handle DOT ticks
                effect.properties["nextTickTime"] = effect.properties.TryGetValue("nextTickTime", out object time)
                    ? (float)time - deltaTime
                    : dotEffect.tickRate;

                if ((float)effect.properties["nextTickTime"] <= 0)
                {
                    float damage = dotEffect.damagePerSecond *
                                  (dotEffect.scaleWithStacks ? effect.stacks : 1) *
                                  dotEffect.tickRate;

                    selfCharacter.lifeComponent.SetDamage(damage);
                    ParticleManager.Instance.PlayDOTParticleEffect(selfCharacter.transform);
                    Debug.Log($"{selfCharacter.name} took {damage} damage from {dotEffect.name}");
                    effect.properties["nextTickTime"] = dotEffect.tickRate;
                }
            }
        }

        // Update visual appearance based on effects
        UpdateVisualEffects();
    }

    /// <summary>
    /// Check if character is in light radius and apply active effects
    /// </summary>
    public void CheckForEffectsInLight()
    {
        if (EffectsManager.Instance == null || selfCharacter.CharacterType != CharacterType.Enemy)
            return;

        Vector3 lightPosition = GameManager.Instance.LightController.hit.point;
        float radius = GameManager.Instance.UpgradeManager.Radius;

        float distance = Vector3.Distance(lightPosition, selfCharacter.transform.position);

        if (distance < radius)
        {
            // Apply all active effects from the effects manager
            foreach (Effect effect in EffectsManager.Instance.ActiveEffectTypes)
            {
                // Apply with 1 stack by default, not distance-based
                AddEffect(effect, 1);
            }
        }
    }

    /// <summary>
    /// Add an effect to this character
    /// </summary>
    public void AddEffect(Effect effect, int stacks = 1)
    {
        if (effect == null || stacks <= 0)
            return;

        // Check if this effect already exists
        ActiveEffect existingEffect = activeEffects.Find(e => e.effect == effect);

        if (existingEffect != null)
        {
            // Effect already exists, refresh or stack
            int oldStacks = existingEffect.stacks;

            if (effect.canStack)
            {
                existingEffect.stacks = Mathf.Min(existingEffect.stacks + stacks, effect.maxStacks);
            }

            existingEffect.remainingDuration = effect.duration;

            // Apply the effect based on its type
            //ApplyEffectImpact(existingEffect, true);
        }
        else
        {
            // Create a new effect instance
            ActiveEffect newEffect = new ActiveEffect(effect, stacks);
            activeEffects.Add(newEffect);

            // Track by type for faster lookups
            Type effectType = effect.GetType();
            if (!effectsByType.ContainsKey(effectType))
            {
                effectsByType[effectType] = new List<ActiveEffect>();
            }
            effectsByType[effectType].Add(newEffect);

            // Spawn visual effect if specified
            if (effect.effectParticlePrefab != null)
            {
                newEffect.visualEffectInstance = GameObject.Instantiate(
                    effect.effectParticlePrefab,
                    selfCharacter.transform.position,
                    Quaternion.identity,
                    selfCharacter.transform
                );
            }

            // Apply the effect based on its type
            //ApplyEffectImpact(newEffect, false);
        }
    }

    /// <summary>
    /// Remove an effect from this character
    /// </summary>
    public void RemoveEffect(Effect effect)
    {
        if (effect == null)
            return;

        // Find the effect instance
        ActiveEffect activeEffect = activeEffects.Find(e => e.effect == effect);

        if (activeEffect != null)
        {
            // Remove effect impact
            //RemoveEffectImpact(activeEffect);

            // Destroy any visual effect
            if (activeEffect.visualEffectInstance != null)
            {
                GameObject.Destroy(activeEffect.visualEffectInstance);
            }

            // Remove from type tracking
            Type effectType = effect.GetType();
            if (effectsByType.ContainsKey(effectType))
            {
                effectsByType[effectType].Remove(activeEffect);
                if (effectsByType[effectType].Count == 0)
                {
                    effectsByType.Remove(effectType);
                }
            }

            // Remove from main list
            activeEffects.Remove(activeEffect);
        }
    }
    public void RemoveAllEffects()
    {
        // Copy the list to avoid modification during iteration
        List<ActiveEffect> effectsCopy = new List<ActiveEffect>(activeEffects);

        foreach (ActiveEffect effect in effectsCopy)
        {
            RemoveEffect(effect.effect);
        }

        // Reset visual appearance
        ResetVisualEffects();
    }

    public bool HasEffect(Effect effect)
    {
        if (effect == null)
            return false;

        return activeEffects.Exists(e => e.effect == effect);
    }

    public bool HasEffectOfType<T>() where T : Effect
    {
        Type effectType = typeof(T);
        return effectsByType.ContainsKey(effectType) && effectsByType[effectType].Count > 0;
    }

    public List<ActiveEffect> GetActiveEffects()
    {
        return new List<ActiveEffect>(activeEffects);
    }

    private void UpdateVisualEffects()
    {
        Renderer renderer = selfCharacter.GetComponentInChildren<Renderer>();
        if (renderer == null)
            return;

        if (activeEffects.Count > 0)
        {
            // Choose the effect with highest priority (for now, just use the first one)
            Effect primaryEffect = activeEffects[0].effect;

            // Apply color tint
            renderer.material.color = primaryEffect.effectColor;
        }
        else
        {
            // Restore original color
            ResetVisualEffects();
        }
    }
    private void ResetVisualEffects()
    {
        Renderer renderer = selfCharacter.GetComponentInChildren<Renderer>();
        if (renderer != null && originalMaterial != null)
        {
            renderer.material.color = originalColor;
        }
    }
}
