using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DefeatWindow : Window
{
    [SerializeField] private Button returnButton;
    [SerializeField] private Button restartButton;

    public override void Initialize()
    {
        returnButton.onClick.AddListener(ReturnHandler);
        restartButton.onClick.AddListener(RestartHandler);
    }
    protected override void OpenStart()
    {
        base.OpenStart();
        OpenEnd();
    }
    protected override void OpenEnd()
    {
        base.OpenEnd();
        returnButton.interactable = true;
        restartButton.interactable = true;
    }
    protected override void CloseStart()
    {
        base.CloseStart();
        returnButton.interactable = false;
        restartButton.interactable = false;
        CloseEnd();
    }
    private void ReturnHandler()
    {
        Hide(false);
        GameManager.Instance.WindowService.ShowWindow<MainMenuWindow>(false);
    }
    private void RestartHandler()
    {
        GameManager.Instance.Restart();
        Hide(false);
        GameManager.Instance.WindowService.ShowWindow<GamePlayWindow>(false);
    }
}
