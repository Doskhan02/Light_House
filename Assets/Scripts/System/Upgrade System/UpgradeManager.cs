using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeManager : MonoBehaviour
{

    private LightData LightData;

    private float currentDamage;
    private float currentRadius;
    private float currentAttackRate;
    private bool upgradeSuccessful;

    public float Damage => currentDamage;
    public float Radius => currentRadius;
    public float AttackRate => currentAttackRate;
    public bool UpgradeSuccessful => upgradeSuccessful;


    private int upgradeCost;
    public void Initialize()
    {
        
    }
    void Start()
    {
        LightData = GameManager.Instance.LightController.LightData;
        currentDamage = LightData.baseDamage;
        currentRadius = LightData.baseRadius;
        currentAttackRate = LightData.baseAttackRate;
        Debug.Log("D: " + currentDamage);
        Debug.Log("R: " + currentRadius);
        Debug.Log("AR: " + currentAttackRate);
    }

    public void ApplyUpgrades(int level, BaseStatUpgrade upgrade)
    {
        upgradeCost = Mathf.RoundToInt(upgrade.cost * Mathf.Pow(upgrade.multiplier, level));

        if (CurrencySystem.Instance.SpendCurrency(upgradeCost) && level < upgrade.maxLevel)
        {
            if (upgrade.upgradeName == "Damage Increase")
            {
                currentDamage = LightData.baseDamage + (upgrade.incrementPerLevel * level);
                upgradeSuccessful = true;
                Debug.Log("D: " + currentDamage);
            }
            if (upgrade.upgradeName == "Radius Increase")
            {
                currentRadius = LightData.baseRadius + (upgrade.incrementPerLevel * level);
                upgradeSuccessful = true;
                Debug.Log("R: " + currentRadius);
            }
            if (upgrade.upgradeName == "Attack Rate Increase")
            {
                currentAttackRate = LightData.baseAttackRate + (upgrade.incrementPerLevel * level);
                upgradeSuccessful = true;
                Debug.Log("AR: " + currentAttackRate);
            }
        }
        else
        {
            Debug.Log("Not enough money");
            upgradeSuccessful = false;
        }
    }
}
