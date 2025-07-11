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
    
    private SelectableBooster currentSelectedBooster;
    
    public override void Initialize()
    {
        if (returnButton != null)
            returnButton.onClick.AddListener(ReturnHandler);
        if (continueButton != null)
            continueButton.onClick.AddListener(ContinueHandler);
        LevelManager.Instance.OnLevelChanged += BlockContinueButton;
            
        // Initialize all boosters
        foreach (var booster in boosters)
        {
            if (booster != null)
            {
                booster.Reinitialize();
                // Подписываемся на событие выбора бустера
                booster.OnBoosterSelected += OnBoosterSelected;
            }
        }
    }
    
    private void OnBoosterSelected(SelectableBooster selectedBooster)
    {
        // Снимаем выбор с других бустеров
        foreach (var booster in boosters)
        {
            if (booster != null && booster != selectedBooster)
            {
                booster.SetSelected(false);
            }
        }
        
        currentSelectedBooster = selectedBooster;
        RefreshContinueButton();
    }
    
    protected override void OpenStart()
    {
        base.OpenStart();
        
        // Сбрасываем выбор при открытии окна
        currentSelectedBooster = null;
        foreach (var booster in boosters)
        {
            if (booster != null)
            {
                booster.SetSelected(false);
            }
        }
        
        RefreshContinueButton();
        OpenEnd();
    }

    private void BlockContinueButton(int level)
    {
        if (level % 6 == 0)
        {
            continueButton.interactable = false;
        }
    }
    
    protected override void CloseStart()
    {
        base.CloseStart();
        if (continueButton != null)
            continueButton.interactable = false;
        CloseEnd();
    }
    
    private void RefreshContinueButton()
    {
        if (continueButton != null)
        {
            continueButton.interactable = !requireSelection || HasValidSelection();
        }
        BlockContinueButton(LevelManager.Instance.CurrentLevel);
    }
    
    private bool HasValidSelection()
    {
        return currentSelectedBooster != null && currentSelectedBooster.IsValid;
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
        return currentSelectedBooster;
    }
    
    private void OnDestroy()
    {
        // Отписываемся от событий
        foreach (var booster in boosters)
        {
            if (booster != null)
            {
                booster.OnBoosterSelected -= OnBoosterSelected;
            }
        }
    }
}