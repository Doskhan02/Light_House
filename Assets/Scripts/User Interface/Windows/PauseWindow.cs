using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class PauseWindow : Window
{
    [SerializeField] private Button returnButton;
    [SerializeField] private Button returnToMainMenuButton;
    [SerializeField] private Toggle soundOnOffButton;
    [SerializeField] private Toggle musicOnOffButton;
    [SerializeField] private Slider soundSlider;
    [SerializeField] private Slider musicSlider;

    public override void Initialize()
    {
        returnButton.onClick.AddListener(ReturnHandler);
        returnToMainMenuButton.onClick.AddListener(ReturnToMainMenuHandler);
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
        soundOnOffButton.interactable = true;
        musicOnOffButton.interactable = true;
        soundSlider.interactable = true;
        musicSlider.interactable = true;
    }
    protected override void CloseStart()
    {
        base.CloseStart();
        returnButton.interactable = false;
        soundOnOffButton.interactable = false;
        musicOnOffButton.interactable = false;
        soundSlider.interactable = false;
        musicSlider.interactable = false;
        CloseEnd();
    }
    private void ReturnHandler()
    {
        Hide(false);
        GameManager.Instance.GameResume();
        GameManager.Instance.WindowService.ShowWindow<GamePlayWindow>(false);
    }
    private void ReturnToMainMenuHandler()
    {
        GameManager.Instance.ReturnToMainMenu();
        Hide(false);
    }
}
