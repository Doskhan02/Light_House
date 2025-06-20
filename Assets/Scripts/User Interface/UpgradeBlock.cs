using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeBlock : MonoBehaviour
{
    [SerializeField] BaseStatUpgrade upgrade;

    public BaseStatUpgrade Upgrade => upgrade;

    [SerializeField] private Button purchaseButton;
    [SerializeField] private Image icon;
    [SerializeField] private TMP_Text upgradeName;
    [SerializeField] private TMP_Text upgradeDescription;
    [SerializeField] private TMP_Text upgradeLevel;
    [SerializeField] private TMP_Text upgradeCost;

    public int level;
    private bool isNew;

    public void Initialize()
    {
        isNew = true;
        purchaseButton.onClick.RemoveAllListeners();
        purchaseButton.onClick.AddListener(PurchaseUpgrade);
        icon.sprite = upgrade.image;
        upgradeName.text = upgrade.upgradeName;
        upgradeDescription.text = upgrade.upgradeDescription;
        upgradeCost.text = Mathf.RoundToInt(upgrade.cost * Mathf.Pow(upgrade.multiplier, level)).ToString();
        UpdateUI();
    }

    private void PurchaseUpgrade()
    {
        if ((level + 1) >= upgrade.maxLevel)
        {
            purchaseButton.interactable = false;
            return;
        }
        GameManager.Instance.UpgradeManager.ApplyUpgrades(level + 1, upgrade, isNew);
        if (GameManager.Instance.UpgradeManager.UpgradeSuccessful)
        {
            level++;
            UpdateUI();
        }
    }
    private void UpdateUI()
    {
        upgradeLevel.text = (level + 1).ToString();
        upgradeCost.text = Mathf.RoundToInt(upgrade.cost * Mathf.Pow(upgrade.multiplier, (level + 1))).ToString();
        purchaseButton.interactable = CurrencySystem.Instance.GetCurrency() >=
                                      Mathf.RoundToInt(upgrade.cost * Mathf.Pow(upgrade.multiplier, (level + 1)));
    }
}
