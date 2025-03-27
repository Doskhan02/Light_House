using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectsManager : MonoBehaviour
{
    [Header("Available Effects")]
    [SerializeField] private List<Effect> availableEffects = new List<Effect>();

    [Header("Unlocked Effects")]
    [SerializeField] private List<Effect> unlockedEffects = new List<Effect>();

    [Header("Active Effect Limit")]
    [SerializeField] private int maxActiveEffects = 3;

    [Header("Active Effect Display")]
    [SerializeField] private Transform effectIconsParent;
    [SerializeField] private GameObject effectIconPrefab;

    private List<Effect> _activeEffectTypes = new List<Effect>();
    public List<Effect> ActiveEffectTypes => _activeEffectTypes;

    public event Action<Effect> OnEffectActivated;
    public event Action<Effect> OnEffectDeactivated;

    public static EffectsManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        for (int i = 0; i < availableEffects.Count; i++)
        {
            ActivateEffect(availableEffects[i]);
        }
        //UpdateEffectIcons();
    }

    public void UnlockEffect(Effect effect)
    {
        if (!unlockedEffects.Contains(effect))
        {
            unlockedEffects.Add(effect);
            //UpdateEffectIcons();
        }
    }

    public bool ActivateEffect(Effect effect)
    {
        if (!unlockedEffects.Contains(effect))
        {
            Debug.LogWarning($"Cannot activate effect {effect.name}: not unlocked");
            return false;
        }

        if (_activeEffectTypes.Contains(effect))
        {
            Debug.Log($"Effect {effect.name} is already active");
            return true;
        }

        if (_activeEffectTypes.Count >= maxActiveEffects)
        {
            Debug.LogWarning($"Cannot activate effect {effect.name}: maximum active effects reached");
            return false;
        }

        _activeEffectTypes.Add(effect);
        OnEffectActivated?.Invoke(effect);
        //UpdateEffectIcons();

        return true;
    }

    public void DeactivateEffect(Effect effect)
    {
        if (_activeEffectTypes.Contains(effect))
        {
            _activeEffectTypes.Remove(effect);
            OnEffectDeactivated?.Invoke(effect);
            //UpdateEffectIcons();
        }
    }
    public void DeactivateAllEffects()
    {
        foreach (Effect effect in _activeEffectTypes)
        {
            DeactivateEffect(effect);
        }
    }

    public void ActivateAllEffects()
    {
        foreach (Effect effect in unlockedEffects)
        {
            ActivateEffect(effect);
        }
    }

    /*
    private void UpdateEffectIcons()
    {
        if (effectIconsParent == null || effectIconPrefab == null)
            return;

        // Clear existing icons
        foreach (Transform child in effectIconsParent)
        {
            Destroy(child.gameObject);
        }

        // Create icon for each active effect
        foreach (Effect effect in _activeEffectTypes)
        {
            GameObject iconObject = Instantiate(effectIconPrefab, effectIconsParent);
            EffectIcon iconComponent = iconObject.GetComponent<EffectIcon>();

            if (iconComponent != null)
            {
                iconComponent.SetEffect(effect);
            }
        }

        // Create faded icons for unlocked but inactive effects
        foreach (Effect effect in unlockedEffects)
        {
            if (!_activeEffectTypes.Contains(effect))
            {
                GameObject iconObject = Instantiate(effectIconPrefab, effectIconsParent);
                EffectIcon iconComponent = iconObject.GetComponent<EffectIcon>();

                if (iconComponent != null)
                {
                    iconComponent.SetEffect(effect, false);
                }
            }
        }
    }*/
}
public class ActiveEffect
{
    public Effect effect;
    public int stacks;
    public float remainingDuration;
    public Dictionary<string, object> properties;
    public GameObject visualEffectInstance;

    public ActiveEffect(Effect effect, int initialStacks)
    {
        this.effect = effect;
        this.stacks = initialStacks;
        this.remainingDuration = effect.duration;
        this.properties = new Dictionary<string, object>();
    }
}
