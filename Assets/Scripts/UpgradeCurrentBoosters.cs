using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeCurrentBoosters : MonoBehaviour
{
    [SerializeField] private Toggle ActiveBoosterToggle;
    [SerializeField] private Toggle PassiveBoosterToggle1;
    [SerializeField] private Toggle PassiveBoosterToggle2;
    
    [SerializeField] private List<SelectableBooster> boosters;

    public void ActivateSelectedBooster()
    {
        var booster = GetSelectedBooster();
        if (booster != null && booster.IsActiveBooster)
        {
            ActiveBoosterManager.Instance.ChooseActiveBooster(booster.currentActiveBooster);
            ActiveBoosterToggle.isOn = true;
            PassiveBoosterToggle1.isOn = false;
            PassiveBoosterToggle2.isOn = false;
        }
        else if(booster != null && !booster.IsActiveBooster)
        {
            int index = CheckEffect(booster.currentEffect);
            
            ActiveBoosterToggle.isOn = false;
            if (PassiveBoosterToggle1.isOn || PassiveBoosterToggle2.isOn)
            {
                EffectsManager.Instance.ActivateEffect(booster.currentEffect);
            }
        }
    }

    private SelectableBooster GetSelectedBooster()
    {
        SelectableBooster selectableBooster = null;
        foreach (var booster in boosters)
        {
            if (booster.IsSelected)
            {
                selectableBooster = booster;
            }
        }
        return selectableBooster;
    }

    private int CheckEffect(Effect effect)
    {
        int index = 0;
        if (EffectsManager.Instance.ActiveEffectTypes.Count == 1)
        {
            return 0;
        }
        if (EffectsManager.Instance.ActiveEffectTypes.Contains(effect))
        {
            index = EffectsManager.Instance.ActiveEffectTypes.IndexOf(effect);
        }

        return index;
    }
}
