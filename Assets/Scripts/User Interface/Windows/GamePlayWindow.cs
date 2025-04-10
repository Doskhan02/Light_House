using TMPro;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UI;

public class GamePlayWindow : Window
{
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private TMP_Text timeText;
    [SerializeField] private Button effectOnButton;
    [SerializeField] private Button effectOffButton;
    [SerializeField] private Button pauseButton;


    public override void Initialize()
    {
        base.Initialize();
        pauseButton.onClick.AddListener(PauseHandler);
        scoreText.text = 0 + " / " + GameManager.Instance.gameData.targetScore;
    }
    protected override void OpenStart()
    {
        base.OpenStart();
        OpenEnd();
    }

    protected override void OpenEnd()
    {
        base.OpenEnd();
        effectOffButton.interactable = true;
        effectOnButton.interactable = true;
        ScoreSystem.Instance.OnScoreUpdated += OnScoreChanged;
        GameManager.Instance.OnSessionTimeUpdated += OnSessionTimeChanged;
    }

    protected override void CloseStart()
    {
        effectOffButton.interactable = false;
        effectOnButton.interactable = false;
        base.CloseStart();
        ScoreSystem.Instance.OnScoreUpdated -= OnScoreChanged;
        GameManager.Instance.OnSessionTimeUpdated -= OnSessionTimeChanged;
        CloseEnd();
    }

    private void OnScoreChanged(int score)
    {
        scoreText.text = score + " / " + GameManager.Instance.gameData.targetScore;
    }

    private void OnSessionTimeChanged(int seconds, int minutes)
    {
        timeText.text = string.Format("{0:D2}:{1:D2}", minutes, seconds);
    }

    private void PauseHandler()
    {
        GameManager.Instance.GamePause();
        Hide(false);
        GameManager.Instance.WindowService.ShowWindow<PauseWindow>(false);
    }

}
