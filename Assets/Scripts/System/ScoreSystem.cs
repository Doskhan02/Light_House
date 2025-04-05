using System;
using UnityEngine;

public class ScoreSystem : MonoBehaviour
{
    public event Action<int> OnScoreUpdated;

    public int Score { get; private set; }

    public void StartGame()
    {
        Score = 0;
    }
    public void AddScore(int earnedScore)
    {
        Score += earnedScore;
        OnScoreUpdated?.Invoke(Score);
    }
}
