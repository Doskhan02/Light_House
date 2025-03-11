using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeBlock : MonoBehaviour
{
    [SerializeField] BaseStatUpgrade upgrade;


    [SerializeField] private Button purchaseButton;
    [SerializeField] private Image icon;
    [SerializeField] private TMP_Text upgradeName;
    [SerializeField] private TMP_Text upgradeDescription;
    [SerializeField] private TMP_Text upgradeLevel;
    [SerializeField] private TMP_Text upgradeCost;

    private int level = 1;


    void Start()
    {
        PlayerPrefs.GetInt(upgradeName.text, 0);
        purchaseButton.onClick.RemoveAllListeners();
        purchaseButton.onClick.AddListener(PurchaseUpgrade);
        icon.sprite = upgrade.image;
        upgradeName.text = upgrade.upgradeName;
        upgradeDescription.text = upgrade.upgradeDescription;
        UpdateUI();
    }

    private void PurchaseUpgrade()
    {
        GameManager.Instance.UpgradeManager.ApplyUpgrades(level, upgrade);
        if (GameManager.Instance.UpgradeManager.UpgradeSuccessful)
        {
            level++;
        }
        UpdateUI();

    }
    private void UpdateUI()
    {
        upgradeLevel.text = level.ToString();
        if (CurrencySystem.Instance.GetCurrency() > Mathf.RoundToInt(upgrade.cost * Mathf.Pow(upgrade.multiplier, level)))
        {
            upgradeCost.text = Mathf.RoundToInt(upgrade.cost * Mathf.Pow(upgrade.multiplier, level)).ToString();
        }
        
    }
    private void OnApplicationQuit()
    {
        PlayerPrefs.SetInt(upgradeName.text, level);
    }
}
