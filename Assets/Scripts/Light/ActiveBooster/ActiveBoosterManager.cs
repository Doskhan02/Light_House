using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActiveBoosterManager : MonoBehaviour
{
    [Header("List of all active booster")]
    [SerializeField] private List<ActiveBooster> activeBoosters;

    private List<ActiveBooster> unlockedActiveBoosters;
    private ActiveBooster currentActiveBooster;

    public ActiveBooster CurrentActiveBooster => currentActiveBooster;

    private float timeSinceLastUse = 0f; // Time since the last booster was used

    public event Action<float> OnCooldownUpdated; // Event to notify cooldown updates
    public event Action<bool> OnBoosterStateChanged; // Event to notify when a booster is applied

    public static ActiveBoosterManager Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
        unlockedActiveBoosters = new List<ActiveBooster>();
        currentActiveBooster = null;
        foreach (var booster in activeBoosters)
        {
            UnlockBooster(booster);
        }
        currentActiveBooster = unlockedActiveBoosters[0]; // Set the first unlocked booster as the current one
    }

    public void UnlockBooster(ActiveBooster booster)
    {
        if (!unlockedActiveBoosters.Contains(booster))
        {
            unlockedActiveBoosters.Add(booster);
        }
        else
        {
            Debug.LogWarning("Booster already unlocked: " + booster.name);
        }
    }

    public void ChooseActiveBooster(ActiveBooster booster)
    {
        if (unlockedActiveBoosters.Contains(booster))
        {
            currentActiveBooster = booster;
        }
        else 
        { 
            Debug.LogWarning("Booster not unlocked yet: " + booster.name); 
        }
    }
    public void ApplyBooster()
    {
        if (currentActiveBooster != null)
        {
            if(currentActiveBooster.boosterType == BoosterType.Global)
            {
                if (timeSinceLastUse > 0)
                {
                    Debug.LogWarning("Booster is on cooldown. Time remaining: " + (currentActiveBooster.cooldown - timeSinceLastUse));
                    return;
                }
                // Apply the booster to all characters
                OnBoosterStateChanged?.Invoke(true); // Notify that the booster is being applied
                List<Character> allCharacters = CharacterSpawnSystem.Instance.CharacterFactory.GetActiveCharacters(CharacterType.Enemy);
                currentActiveBooster.ApplyBooster(allCharacters);
                timeSinceLastUse = currentActiveBooster.cooldown; // Reset the cooldown timer
                Debug.Log("Applied global booster to all enemy type characters.");

                return;
            }
            else
            {
                currentActiveBooster.ApplyBooster();
            }
        }
        else
        {
            Debug.LogWarning("No active booster selected.");
        }
    }
    private void Update()
    {
        if (currentActiveBooster != null && timeSinceLastUse >= 0)
        {
            OnCooldownUpdated?.Invoke(timeSinceLastUse); // Notify the cooldown update
            timeSinceLastUse -= Time.deltaTime; // Increment the cooldown timer
            if(timeSinceLastUse <= 0)
            {
                timeSinceLastUse = 0; // Reset the cooldown timer
                OnBoosterStateChanged?.Invoke(false); // Notify that the booster is no longer applied
            }
        }
    }
}
