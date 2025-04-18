using System;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeManager : MonoBehaviour, IDataPersistance
{
    [SerializeField] private List<UpgradeBlock> upgrades;
    private LightData LightData;

    public event Action<float, float, float> OnUpgradeApplied;

    private int damageLevel;
    private int radiusLevel;
    private int attckRateLevel;

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
        foreach (UpgradeBlock upgrade in upgrades) 
        {
            if (upgrade.Upgrade.upgradeName == "Damage Increase")
            {
                upgrade.level = damageLevel;
                ApplyUpgrades(damageLevel, upgrade.Upgrade, false);
            }
            else if (upgrade.Upgrade.upgradeName == "Radius Increase")
            {
                upgrade.level = radiusLevel;
                ApplyUpgrades(radiusLevel, upgrade.Upgrade, false);
            }
            else if (upgrade.Upgrade.upgradeName == "Attack Rate Increase")
            {
                upgrade.level = attckRateLevel;
                ApplyUpgrades(attckRateLevel, upgrade.Upgrade, false);
            }
            upgrade.Initialize();
        }

    }
    void Start()
    {
        LightData = GameManager.Instance.LightController.LightData;
        currentDamage = LightData.baseDamage;
        currentRadius = LightData.baseRadius;
        currentAttackRate = LightData.baseAttackRate;
        Initialize();
    }

    public void ApplyUpgrades(int level, BaseStatUpgrade upgrade, bool isNew)
    {
        if (isNew)
        {
            upgradeCost = Mathf.RoundToInt(upgrade.cost * Mathf.Pow(upgrade.multiplier, level));
        }
        else 
        { 
            upgradeCost = 0; 
        }

        if (CurrencySystem.Instance.SpendCurrency(upgradeCost) && level < upgrade.maxLevel)
        {
            if (upgrade.upgradeName == "Damage Increase")
            {
                currentDamage = LightData.baseDamage + (upgrade.incrementPerLevel * level);
                damageLevel = level;
                upgradeSuccessful = true;
            }
            if (upgrade.upgradeName == "Radius Increase")
            {
                currentRadius = LightData.baseRadius + (upgrade.incrementPerLevel * level);
                radiusLevel = level;
                upgradeSuccessful = true;
            }
            if (upgrade.upgradeName == "Attack Rate Increase")
            {
                currentAttackRate = LightData.baseAttackRate - (upgrade.incrementPerLevel * level);
                attckRateLevel = level;
                upgradeSuccessful = true;
            }
            OnUpgradeApplied?.Invoke(currentDamage, currentRadius, currentAttackRate);
        }
        else
        {
            Debug.Log("Not enough money");
            upgradeSuccessful = false;
        }
    }

    public void LoadData(GamePersistantData data)
    {
        damageLevel = data.damageUpgradeLevel;
        radiusLevel = data.radiusUpgradeLevel;
        attckRateLevel = data.attackRateUpgradeLevel;
    }

    public void SaveData(ref GamePersistantData data)
    {
        data.damageUpgradeLevel = damageLevel;
        data.radiusUpgradeLevel = radiusLevel;
        data.attackRateUpgradeLevel = attckRateLevel;
    }
}
