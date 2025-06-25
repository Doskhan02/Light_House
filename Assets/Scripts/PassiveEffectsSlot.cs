using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PassiveEffectsSlot : MonoBehaviour
{
    [SerializeField] private Image iconImage;
    [SerializeField] private int index;
    private EffectsManager effectsManager;
    private Effect effect;
    
    private void Start()
    {
        effectsManager = EffectsManager.Instance;
        if (effectsManager != null && effectsManager.ActiveEffectTypes.Count > 0)
        {
            effect = effectsManager.ActiveEffectTypes[index];
            iconImage.sprite = effect.icon;
        }
        else
        {
            iconImage.color = Color.clear;
            iconImage.sprite = null;
        }

        effectsManager.OnEffectActivated += OnEffectChanged;
    }

    private void OnEffectChanged(Effect newEffect)
    {
        if (newEffect != effect && !effectsManager.ActiveEffectTypes.Contains(effect))
        {
            effect = newEffect;
            iconImage.sprite = effect.icon;
            iconImage.color = Color.white;
        }
    }
}
