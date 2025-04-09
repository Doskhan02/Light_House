using System;
using UnityEngine;

public class CurrencySystem : MonoBehaviour, IDataPersistance
{
    public static CurrencySystem Instance;

    private int softCurrency;


    public event Action<int> OnCurrencyChanged;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
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
    }

    public bool SpendCurrency(int amount)
    {
        if (softCurrency >= amount)
        {
            softCurrency -= amount;
            OnCurrencyChanged?.Invoke(softCurrency);
            return true;
        }
        return false;
    }

    public void LoadData(GamePersistantData data)
    {
        softCurrency= data.softCurrency;
    }

    public void SaveData(ref GamePersistantData data)
    {
        data.softCurrency = softCurrency;
    }
}
