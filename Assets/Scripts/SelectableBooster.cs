using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SelectableBooster : MonoBehaviour
{
    [SerializeField] private Toggle toggle;
    [SerializeField] private Image iconImage;
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text descriptionText;
    
    private ActiveBoosterManager activeBoosterManager;
    private EffectsManager effectsManager;
    
    private List<Effect> availableEffects;
    private List<ActiveBooster> availableActiveBoosters;
    
    private bool isActiveBooster;
    
    public Effect currentEffect { get; private set; }
    public ActiveBooster currentActiveBooster { get; private set; }
    
    public bool IsSelected => toggle != null && toggle.isOn;
    public bool IsActiveBooster => isActiveBooster;
    public bool IsValid => (isActiveBooster && currentActiveBooster != null) || (!isActiveBooster && currentEffect != null);
    
    private void Start()
    {
        activeBoosterManager = ActiveBoosterManager.Instance;
        effectsManager = EffectsManager.Instance;
        
        RefreshAvailableOptions();
        Initialize();
    }
    
    private void RefreshAvailableOptions()
    {
        availableActiveBoosters = activeBoosterManager?.UnlockedActiveBoosters ?? new List<ActiveBooster>();
        availableEffects = effectsManager?.UnlockedEffects ?? new List<Effect>();
    }
    
    private void Initialize()
    {
        // Ensure we have options available
        if (availableActiveBoosters.Count == 0 && availableEffects.Count == 0)
        {
            Debug.LogWarning("No available boosters or effects to initialize SelectableBooster");
            SetInvalidState();
            return;
        }
        
        // Decide type based on availability and randomness
        if (availableActiveBoosters.Count == 0)
        {
            isActiveBooster = false;
        }
        else if (availableEffects.Count == 0)
        {
            isActiveBooster = true;
        }
        else
        {
            isActiveBooster = Random.value < 0.5f;
        }
        
        if (isActiveBooster)
        {
            InitializeAsActiveBooster();
        }
        else
        {
            InitializeAsEffect();
        }
    }
    
    private void InitializeAsActiveBooster()
    {
        if (availableActiveBoosters.Count > 0)
        {
            currentActiveBooster = availableActiveBoosters[Random.Range(0, availableActiveBoosters.Count)];
            UpdateUI(currentActiveBooster.icon, currentActiveBooster.name, currentActiveBooster.description);
        }
        else
        {
            SetInvalidState();
        }
    }
    
    private void InitializeAsEffect()
    {
        if (availableEffects.Count > 0)
        {
            currentEffect = availableEffects[Random.Range(0, availableEffects.Count)];
            UpdateUI(currentEffect.icon, currentEffect.name, currentEffect.description);
        }
        else
        {
            SetInvalidState();
        }
    }
    
    private void UpdateUI(Sprite icon, string itemName, string description)
    {
        if (iconImage != null) iconImage.sprite = icon;
        if (nameText != null) nameText.text = itemName ?? "Unknown";
        if (descriptionText != null) descriptionText.text = description ?? "No description available";
    }
    
    private void SetInvalidState()
    {
        currentEffect = null;
        currentActiveBooster = null;
        UpdateUI(null, "Invalid", "No options available");
        
        if (toggle != null)
        {
            toggle.interactable = false;
        }
    }
    
    public ActiveBooster GetActiveBooster() => currentActiveBooster;
    public Effect GetCurrentEffect() => currentEffect;
    
    // Method to reinitialize if needed
    public void Reinitialize()
    {
        RefreshAvailableOptions();
        Initialize();
    }
}
