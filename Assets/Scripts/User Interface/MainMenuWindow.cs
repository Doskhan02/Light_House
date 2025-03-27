using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuWindow : Window
{
    [SerializeField] private Button startGameButton;
    [SerializeField] private Button optionsGameButton;
    public override void Initialize()
    {
        startGameButton.onClick.AddListener(StartGameHandler);
        optionsGameButton.onClick.AddListener(OpenOptionsHandler);
    }
    protected override void OpenEnd()
    {
        base.OpenEnd();
        startGameButton.interactable = true;
        optionsGameButton.interactable = true;
    }
    protected override void CloseStart()
    {
        base.CloseStart();
        startGameButton.interactable = false;
        optionsGameButton.interactable = false;
    }
    private void StartGameHandler()
    {
        GameManager.Instance.StartGame();
        GameManager.Instance.WindowService.ShowWindow<GamePlayWindow>(true);
        Hide(false);
    }
    private void OpenOptionsHandler()
    {
        Hide(false);
        GameManager.Instance.WindowService.ShowWindow<OptionsWindow>(true);
    }
}
