using System;
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
        /*for (int i = 0; i < availableEffects.Count; i++)
        {
            ActivateEffect(availableEffects[i]);
        }*/
    }

    public void UnlockEffect(Effect effect)
    {
        if (!unlockedEffects.Contains(effect))
        {
            unlockedEffects.Add(effect);
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

        return true;
    }

    public void DeactivateEffect(Effect effect)
    {
        if (_activeEffectTypes.Contains(effect))
        {
            _activeEffectTypes.Remove(effect);
            OnEffectDeactivated?.Invoke(effect);
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
