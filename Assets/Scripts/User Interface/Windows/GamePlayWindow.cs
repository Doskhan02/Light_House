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
    [SerializeField] private Button activeBoosterButton;
    [SerializeField] private TMP_Text activeBoosterText;


    public override void Initialize()
    {
        base.Initialize();
        pauseButton.onClick.AddListener(PauseHandler);
        scoreText.text = 0 + " / " + GameManager.Instance.gameData.targetScore;
    }
    protected override void OpenStart()
    {
        base.OpenStart();
        scoreText.text = 0 + " / " + GameManager.Instance.gameData.targetScore;
        OpenEnd();
    }

    protected override void OpenEnd()
    {
        base.OpenEnd();
        effectOffButton.interactable = true;
        effectOnButton.interactable = true;
        ScoreSystem.Instance.OnScoreUpdated += OnScoreChanged;
        GameManager.Instance.OnSessionTimeUpdated += OnSessionTimeChanged;
        if (ActiveBoosterManager.Instance != null)
        {
            activeBoosterButton.onClick.AddListener(ActiveBoosterManager.Instance.ApplyBooster);
            activeBoosterButton.image.sprite = ActiveBoosterManager.Instance.CurrentActiveBooster.icon;
            ActiveBoosterManager.Instance.OnBoosterStateChanged += OnSkillActive;
            ActiveBoosterManager.Instance.OnCooldownUpdated += OnActiveBoosterTimerChanged;
        }
        else
        {
            Debug.LogError("ActiveBoosterManager.Instance is null when trying to add listener.");
        }
    }

    protected override void CloseStart()
    {
        effectOffButton.interactable = false;
        effectOnButton.interactable = false;
        base.CloseStart();
        ScoreSystem.Instance.OnScoreUpdated -= OnScoreChanged;
        GameManager.Instance.OnSessionTimeUpdated -= OnSessionTimeChanged;
        activeBoosterButton.onClick.RemoveAllListeners();
        ActiveBoosterManager.Instance.OnBoosterStateChanged -= OnSkillActive;
        ActiveBoosterManager.Instance.OnCooldownUpdated -= OnActiveBoosterTimerChanged;
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

    private void OnSkillActive(bool isActive)
    {
        activeBoosterButton.interactable = !isActive;
        activeBoosterText.gameObject.SetActive(isActive);
    }
    private void OnActiveBoosterTimerChanged(float time)
    {
        activeBoosterText.text = string.Format("{0:F1}", time);
    }
}
