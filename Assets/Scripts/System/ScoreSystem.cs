using System;
using UnityEngine;

public class ScoreSystem : MonoBehaviour
{
    public static ScoreSystem Instance { get; private set; }

    private GameManager gameManager;
    private CurrencySystem currencySystem;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public event Action<int> OnScoreUpdated;

    public event Action<int> OnRewardUpdated;

    public int Score { get; private set; }
    private int Reward { get; set; }

    public void StartGame()
    {
        Instance = this;
        Score = 0;
        OnScoreUpdated?.Invoke(Score);
    }
    public void AddScore(int earnedScore)
    {
        Score += earnedScore;
        if (Score <= 0)
            Score = 0;
        OnScoreUpdated?.Invoke(Score);
    }
    public void CalculateReward()
    {
        int timeInSec = GameManager.Instance.SessionTimeInSeconds;
        int timeInMin = GameManager.Instance.SessionTimeInMinutes;
        GameData data = GameManager.Instance.gameData;
        float maxTimeInSec = data.sessionMaxTimeInMinutes * 60 + data.sessionMaxTimeInSeconds;
        Reward = (int)(Score * 10 + (maxTimeInSec - (timeInSec + timeInMin * 60)) * 10);
        OnRewardUpdated?.Invoke(Reward);
        CurrencySystem.Instance.AddCurrency(Reward);
    }
}
