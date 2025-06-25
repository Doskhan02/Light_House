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
    
    private List<Effect> unlockedEffects;
    private List<ActiveBooster> unlockedActiveBoosters;
    
    private bool isActiveBooster;
    
    public Effect currentEffect;
    public ActiveBooster currentActiveBooster;
    
    public bool IsSelected => toggle.isOn;
    public bool IsActiveBooster => isActiveBooster;
    
    private void Start()
    {
        activeBoosterManager = ActiveBoosterManager.Instance;
        effectsManager = EffectsManager.Instance;
        unlockedActiveBoosters = activeBoosterManager.UnlockedActiveBoosters;
        unlockedEffects = effectsManager.UnlockedEffects;
        Initialize();
    }

    private void Initialize()
    {
        isActiveBooster = Random.value < 0.5f;
        if (isActiveBooster)
        {
            currentActiveBooster = unlockedActiveBoosters[Random.Range(0, unlockedActiveBoosters.Count)];
            iconImage.sprite = currentActiveBooster.icon;
            nameText.text = currentActiveBooster.name;
            descriptionText.text = currentActiveBooster.description;
        }
        else
        {
            currentEffect = unlockedEffects[Random.Range(0, unlockedEffects.Count)];
            iconImage.sprite = currentEffect.icon;
            nameText.text = currentEffect.name;
            descriptionText.text = currentEffect.description;
        }
    }

    public ActiveBooster GetActiveBooster()
    {
        return currentActiveBooster;
    }

    public Effect GetCurrentEffect()
    {
        return currentEffect;
    }
}
