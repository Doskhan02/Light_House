
using UnityEngine;
using UnityEngine.UI;

public class ActiveBoosterSlot : MonoBehaviour
{
    [SerializeField] private Image iconImage;
    [SerializeField] private Color emptySlotColor = Color.clear;
    [SerializeField] private Color activeSlotColor = Color.white;
    
    private ActiveBoosterManager activeBoosterManager;
    private ActiveBooster currentActiveBooster;
    
    private void Start()
    {
        activeBoosterManager = ActiveBoosterManager.Instance;
        if (activeBoosterManager != null)
        {
            activeBoosterManager.OnCurrentActiveBoosterChanged += OnActiveBoosterChanged;
        }
        
        RefreshSlot();
    }
    
    private void OnDestroy()
    {
        if (activeBoosterManager != null)
        {
            activeBoosterManager.OnCurrentActiveBoosterChanged -= OnActiveBoosterChanged;
        }
    }
    
    private void RefreshSlot()
    {
        if (activeBoosterManager != null)
        {
            currentActiveBooster = activeBoosterManager.CurrentActiveBooster;
        }
        
        if (currentActiveBooster != null)
        {
            SetActiveSlot(currentActiveBooster);
        }
        else
        {
            SetEmptySlot();
        }
    }
    
    private void SetActiveSlot(ActiveBooster activeBooster)
    {
        if (iconImage != null && activeBooster != null)
        {
            iconImage.sprite = activeBooster.icon;
            iconImage.color = activeSlotColor;
        }
    }
    
    private void SetEmptySlot()
    {
        if (iconImage != null)
        {
            iconImage.sprite = null;
            iconImage.color = emptySlotColor;
        }
    }
    
    private void OnActiveBoosterChanged(ActiveBooster activeBooster)
    {
        currentActiveBooster = activeBooster;
        RefreshSlot();
    }
}
