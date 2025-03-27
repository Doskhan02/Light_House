using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour, IDataPersistance
{
    public int CurrentLevel { get; private set; } = 1;
    public int CheckPointLevel { get; private set; } = 1;

    private float difficultyMultiplier = 1;

    public event Action<int> OnLevelChanged;
    public void NextLevel()
    {
        CurrentLevel++;
        if ((CurrentLevel -1)% 3 == 0)
        {
            CheckPointSet();
        }
        OnLevelChanged?.Invoke(CurrentLevel);
    }
    
    public void CheckPointSet() 
    {
        CheckPointLevel = CurrentLevel;
    }

    public void CheckPointReset()
    {
        CurrentLevel = CheckPointLevel;
        OnLevelChanged?.Invoke(CurrentLevel);
    }

    public void LoadData(GamePersistantData data)
    {
        CurrentLevel = data.currentGameLevel;
        CheckPointLevel = data.checkpointLevel;
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

