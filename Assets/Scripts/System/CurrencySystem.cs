using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CurrencySystem : MonoBehaviour
{
    public static CurrencySystem Instance;

    private int softCurrency;


    public event Action<int> OnCurrencyChanged;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            LoadCurrency();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public int GetCurrency()
    {
        return softCurrency;
    }

    public void AddCurrency(int amount)
    {
        softCurrency += amount;
        OnCurrencyChanged?.Invoke(softCurrency);
        SaveCurrency();
    }

    public bool SpendCurrency(int amount)
    {
        if (softCurrency >= amount)
        {
            softCurrency -= amount;
            OnCurrencyChanged?.Invoke(softCurrency);
            SaveCurrency();
            return true;
        }
        return false;
    }

    private void SaveCurrency()
    {
        PlayerPrefs.SetInt("SoftCurrency", softCurrency);
        PlayerPrefs.Save();
    }

    private void LoadCurrency()
    {
        softCurrency = PlayerPrefs.GetInt("SoftCurrency", 500); 
    }

}
