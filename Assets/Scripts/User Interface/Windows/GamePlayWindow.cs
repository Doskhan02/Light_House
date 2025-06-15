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
    [SerializeField] private Slider bossHP;

    private Character bossCharacter;

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
        effectOffButton.interactable = true;
        effectOnButton.interactable = true;
        ScoreSystem.Instance.OnScoreUpdated += OnScoreChanged;
        GameManager.Instance.OnSessionTimeUpdated += OnSessionTimeChanged;



        if (GameManager.Instance.LevelManager.CurrentLevel % 6 == 0)
        {
            Invoke(nameof(BossHandler), 20);
        }
        else
        {
            bossHP.gameObject.SetActive(false);
        }

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
        base.OpenEnd();
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

    private void BossHandler()
    {
        if (GameManager.Instance.LevelManager.CurrentLevel%6 == 0)
        {
            if (GameObject.FindGameObjectWithTag("Boss") == null)
            {
                Debug.LogError("Boss GameObject not found in the scene.");
                return;
            }
            bossCharacter = GameObject.FindGameObjectWithTag("Boss").GetComponent<Character>();
            if (bossCharacter == null)
            {
                Debug.LogError("Boss character not found in the scene.");
                return;
            }
            bossHP.gameObject.SetActive(true);
            bossHP.maxValue = bossCharacter.CharacterData.CharacterTypeData.defaultMaxHP;
            bossHP.value = bossCharacter.lifeComponent.Health;
            bossCharacter.lifeComponent.OnCharacterHealthChange += OnBossHealthChanged;
        }
        else
        {
            bossHP.gameObject.SetActive(false);
        }
    }
    private void OnBossHealthChanged(Character character)
    {
        bossHP.value = character.lifeComponent.Health;
    }
}
