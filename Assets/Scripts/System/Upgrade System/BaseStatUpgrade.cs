using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BaseStatUpgrade", menuName = "Scriptable Objects/Upgrade/Base Stat Upgrade")]
public class BaseStatUpgrade : Upgrade
{
    public int maxLevel;

    public float incrementPerLevel;
    public float multiplier;
    public int cost;

}
