using UnityEngine;
using UnityEngine.UI;

public class PassiveEffectsSlot : MonoBehaviour
{
    [SerializeField] private Image iconImage;
    [SerializeField] private int slotIndex;
    [SerializeField] private Color emptySlotColor = Color.clear;
    [SerializeField] private Color activeSlotColor = Color.white;
    
    private EffectsManager effectsManager;
    private Effect currentEffect;
    
    private void Start()
    {
        effectsManager = EffectsManager.Instance;
        if (effectsManager != null)
        {
            // Subscribe to events
            effectsManager.OnEffectActivated += OnEffectActivated;
            effectsManager.OnEffectDeactivated += OnEffectDeactivated;
        }
        
        RefreshSlot();
    }
    
    private void OnDestroy()
    {
        if (effectsManager != null)
        {
            effectsManager.OnEffectActivated -= OnEffectActivated;
            effectsManager.OnEffectDeactivated -= OnEffectDeactivated;
        }
    }
    
    private void RefreshSlot()
    {
        if (effectsManager == null || effectsManager.ActiveEffectTypes == null)
        {
            SetEmptySlot();
            return;
        }
        
        // Check if we have an effect at this slot index
        if (slotIndex < effectsManager.ActiveEffectTypes.Count)
        {
            Effect effect = effectsManager.ActiveEffectTypes[slotIndex];
            SetActiveSlot(effect);
        }
        else
        {
            SetEmptySlot();
        }
    }
    
    private void SetActiveSlot(Effect effect)
    {
        currentEffect = effect;
        if (iconImage != null && effect != null)
        {
            iconImage.sprite = effect.icon;
            iconImage.color = activeSlotColor;
        }
    }
    
    private void SetEmptySlot()
    {
        currentEffect = null;
        if (iconImage != null)
        {
            iconImage.sprite = null;
            iconImage.color = emptySlotColor;
        }
    }
    
    private void OnEffectActivated(Effect newEffect)
    {
        RefreshSlot();
    }
    
    private void OnEffectDeactivated(Effect removedEffect)
    {
        RefreshSlot();
    }
}
