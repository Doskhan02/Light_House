using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour, IDataPersistance
{
    public int CurrentLevel { get; private set; } = 1;
    public int CheckPointLevel { get; private set; } = 1;

    public static LevelManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    private float difficultyMultiplier = 1;

    public event Action<int> OnLevelChanged;
    public event Action OnGameLevelChanged;
    public void NextLevel()
    {
        CurrentLevel++;
        if ((CurrentLevel -1)% 3 == 0)
        {
            CheckPointSet();
        }
        OnLevelChanged?.Invoke(CurrentLevel);
        OnGameLevelChanged?.Invoke();
    }
    
    public void CheckPointSet() 
    {
        CheckPointLevel = CurrentLevel;
    }

    public void CheckPointReset()
    {
        CurrentLevel = CheckPointLevel;
        OnLevelChanged?.Invoke(CurrentLevel);
        OnGameLevelChanged?.Invoke();
    }

    public void LoadData(GamePersistantData data)
    {
        CurrentLevel = data.currentGameLevel;
        CheckPointLevel = data.checkpointLevel;
        OnLevelChanged?.Invoke(CurrentLevel);
    }

    public void SaveData(ref GamePersistantData data)
    {
        data.currentGameLevel = CurrentLevel;
        data.checkpointLevel = CheckPointLevel;
    }

    public void DifficultyIncrease()
    {
        difficultyMultiplier++;
    }
    public float GetDifficultyMultiplier()
    {
        return difficultyMultiplier;
    }
}

