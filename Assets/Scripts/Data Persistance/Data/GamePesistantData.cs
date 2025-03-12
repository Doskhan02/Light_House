using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GamePesistantData
{
    public int softCurrency;
    public int damageUpgradeLevel;
    public int radiusUpgradeLevel;
    public int attackRateUpgradeLevel;

    public GamePesistantData()
    {
        this.softCurrency = 500;
        this.damageUpgradeLevel = 1;
        this.radiusUpgradeLevel = 1;
        this.attackRateUpgradeLevel = 1;
    }

}
