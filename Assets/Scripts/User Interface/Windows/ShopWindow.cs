using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShopWindow : Window
{
    [SerializeField] private Button returnButton;
    [SerializeField] private TMP_Text currency;
    [SerializeField] private TMP_Text currentStatsText;

    public override void Initialize()
    {
        returnButton.onClick.AddListener(ReturnHandler);
    }
    protected override void OpenStart()
    {
        CurrencySystem.Instance.OnCurrencyChanged += CurrencyHandler;
        GameManager.Instance.UpgradeManager.OnUpgradeApplied += CurrentStatsHandler;
        base.OpenStart();
        OpenEnd();
    }
    protected override void OpenEnd()
    {
        base.OpenEnd();
        returnButton.interactable = true;
        currency.text = CurrencySystem.Instance.GetCurrency().ToString();
    }
    protected override void CloseStart()
    {
        CurrencySystem.Instance.OnCurrencyChanged -= CurrencyHandler;
        GameManager.Instance.UpgradeManager.OnUpgradeApplied -= CurrentStatsHandler;
        base.CloseStart();
        returnButton.interactable = false;
        CloseEnd();
    }
    private void ReturnHandler()
    {
        Hide(false);
        GameManager.Instance.WindowService.ShowWindow<MainMenuWindow>(false);
    }
    public void Update()
    {
        currentStatsText.text = "Current Stats:" + "\n"
            + "Damage: " + GameManager.Instance.UpgradeManager.Damage.ToString() + "\n"
            + "Radius: " + GameManager.Instance.UpgradeManager.Radius.ToString() + "\n"
            + "Attack Rate: " + string.Format("{0:f2}", 1 / GameManager.Instance.UpgradeManager.AttackRate);
    }
    private void CurrencyHandler(int currency)
    {
        this.currency.text = currency.ToString();
    }
    private void CurrentStatsHandler(float damage, float radius, float attackRate)
    {
        currentStatsText.text = "Current Stats:" + "\n"
            + "Damage: " + damage.ToString() + "\n"
            + "Radius: " + radius.ToString() + "\n"
            + "Attack Rate: " + (attackRate * 60).ToString();
    }
}
