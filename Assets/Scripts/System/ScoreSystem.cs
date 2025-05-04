using System;
using UnityEngine;

public class ScoreSystem : MonoBehaviour
{
    public static ScoreSystem Instance { get; private set; }

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
    public int Reward { get; private set; }

    public void StartGame()
    {
        Instance = this;
        Score = 0;
    }
    public void AddScore(int earnedScore)
    {
        Score += earnedScore;
        OnScoreUpdated?.Invoke(Score);
        if(Score <= 0)
            Score = 0;
    }
    public void CalculateReward()
    {
        int timeInSec = GameManager.Instance.SessionTimeInSeconds;
        int timeInMin = GameManager.Instance.SessionTimeInMinutes;
        Reward = (int)(Score * 6000 * 1f / (timeInSec + timeInMin * 60));
        OnRewardUpdated?.Invoke(Reward);
        CurrencySystem.Instance.AddCurrency(Reward);
    }
}
