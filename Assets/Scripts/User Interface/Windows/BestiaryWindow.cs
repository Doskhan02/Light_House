using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BestiaryWindow : Window
{
    [SerializeField] private Button returnButton;
    [SerializeField] private Button nextPageButton;
    [SerializeField] private Button previousPageButton;

    [SerializeField] private BestiaryController bestiaryController;

    public override void Initialize()
    {
        returnButton.onClick.AddListener(ReturnHandler);
        nextPageButton.onClick.AddListener(NextPageHandler);
        previousPageButton.onClick.AddListener(PreviousPageHandler);
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
        nextPageButton.interactable = true;
        previousPageButton.interactable = true;
    }
    protected override void CloseStart()
    {
        base.CloseStart();
        returnButton.interactable = false;
        nextPageButton.interactable = false;
        previousPageButton.interactable = false;
        CloseEnd();
    }
    private void ReturnHandler()
    {
        Hide(false);
        GameManager.Instance.WindowService.ShowWindow<MainMenuWindow>(false);
    }
    private void NextPageHandler()
    {
        bestiaryController.NextPage();
    }
    private void PreviousPageHandler()
    {
        bestiaryController.PreviousPage();
    }
}
