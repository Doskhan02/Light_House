using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BoosterSelectWindow : Window
{
    [SerializeField] private Button returnButton;
    [SerializeField] private Button continueButton;
    [SerializeField] private List<SelectableBooster> boosters;
    
    // Optional validation
    [SerializeField] private bool requireSelection = true;
    
    public override void Initialize()
    {
        if (returnButton != null)
            returnButton.onClick.AddListener(ReturnHandler);
        if (continueButton != null)
            continueButton.onClick.AddListener(ContinueHandler);
            
        // Initialize all boosters
        foreach (var booster in boosters)
        {
            if (booster != null)
            {
                booster.Reinitialize();
            }
        }
    }
    
    protected override void OpenStart()
    {
        base.OpenStart();
        RefreshContinueButton();
        OpenEnd();
    }
    
    protected override void OpenEnd()
    {
        base.OpenEnd();
        // Additional setup if needed
    }
    
    protected override void CloseStart()
    {
        base.CloseStart();
        if (continueButton != null)
            continueButton.interactable = false;
        CloseEnd();
    }
    
    private void Update()
    {
        // Continuously check if we should enable/disable continue button
        if (requireSelection)
        {
            RefreshContinueButton();
        }
    }
    
    private void RefreshContinueButton()
    {
        if (continueButton != null)
        {
            continueButton.interactable = !requireSelection || HasValidSelection();
        }
    }
    
    private bool HasValidSelection()
    {
        foreach (var booster in boosters)
        {
            if (booster != null && booster.IsSelected && booster.IsValid)
            {
                return true;
            }
        }
        return false;
    }
    
    private void ReturnHandler()
    {
        ApplySelectedBooster();
        Hide(false);
        GameManager.Instance?.ReturnToMainMenu();
    }
    
    private void ContinueHandler()
    {
        ApplySelectedBooster();
        Hide(false);
        
        var gameManager = GameManager.Instance;
        if (gameManager != null)
        {
            gameManager.GameContinue();
            
            // Show appropriate window based on level
            if (LevelManager.Instance != null && LevelManager.Instance.CurrentLevel % 6 != 0)
            {
                gameManager.WindowService?.ShowWindow<GamePlayWindow>(false);
            }
        }
    }
    
    private void ApplySelectedBooster()
    {
        var selectedBooster = GetSelectedBooster();
        if (selectedBooster == null || !selectedBooster.IsValid)
        {
            Debug.LogWarning("No valid booster selected");
            return;
        }
        
        if (selectedBooster.IsActiveBooster)
        {
            ApplyActiveBooster(selectedBooster.GetActiveBooster());
        }
        else
        {
            ApplyPassiveEffect(selectedBooster.GetCurrentEffect());
        }
    }
    
    private void ApplyActiveBooster(ActiveBooster activeBooster)
    {
        if (activeBooster != null && ActiveBoosterManager.Instance != null)
        {
            ActiveBoosterManager.Instance.ChooseActiveBooster(activeBooster);
            Debug.Log($"Applied active booster: {activeBooster.name}");
        }
    }
    
    private void ApplyPassiveEffect(Effect effect)
    {
        if (effect != null && EffectsManager.Instance != null)
        {
            EffectsManager.Instance.ActivateEffect(effect);
            Debug.Log($"Applied passive effect: {effect.name}");
        }
    }
    
    private SelectableBooster GetSelectedBooster()
    {
        foreach (var booster in boosters)
        {
            if (booster != null && booster.IsSelected)
            {
                return booster;
            }
        }
        return null;
    }
}
