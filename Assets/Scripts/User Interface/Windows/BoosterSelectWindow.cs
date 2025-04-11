using UnityEngine;
using UnityEngine.UI;

public class BoosterSelectWindow : Window
{
    [SerializeField] private Button returnButton;
    [SerializeField] private Button continueButton;
    
    public override void Initialize()
    {
        returnButton.onClick.AddListener(ReturnHandler);
        continueButton.onClick.AddListener(ContinueHandler);
    }
    protected override void OpenStart()
    {
        base.OpenStart();
        OpenEnd();
    }
    protected override void OpenEnd()
    {
        base.OpenEnd();
        continueButton.interactable = true;
    }
    protected override void CloseStart()
    {
        base.CloseStart();
        continueButton.interactable = false;
        CloseEnd();
    }
    private void ReturnHandler()
    {
        Hide(false);
        GameManager.Instance.ReturnToMainMenu();
    }
    private void ContinueHandler()
    {
        GameManager.Instance.GameContinue();
        Hide(false);
        GameManager.Instance.WindowService.ShowWindow<GamePlayWindow>(false);

    }

}
