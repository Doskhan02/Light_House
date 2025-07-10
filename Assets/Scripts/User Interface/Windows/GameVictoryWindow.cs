using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameVictoryWindow : Window
{
    [SerializeField] private Button continueButton;
    [SerializeField] private TMP_Text coinText;
    [SerializeField] private Image characterImage;
    [SerializeField] private TextMeshProUGUI characterNameText;
    [SerializeField] private TextMeshProUGUI characterText;

    public override void Initialize()
    {
        continueButton.onClick.AddListener(ContinueHandler);
    }
    protected override void OpenStart()
    {
        base.OpenStart();
        characterImage.sprite = GameManager.Instance.gameData.characterSprite;
        characterNameText.text = GameManager.Instance.gameData.characterName;
        characterText.text = GameManager.Instance.gameData.characterText;
        OpenEnd();
    }
    protected override void OpenEnd()
    {
        ScoreSystem.Instance.OnRewardUpdated += UpdateReward;
        base.OpenEnd();
        continueButton.interactable = true;
    }
    protected override void CloseStart()
    {
        ScoreSystem.Instance.OnRewardUpdated -= UpdateReward;
        base.CloseStart();
        continueButton.interactable = false;
        CloseEnd();
    }
    private void ContinueHandler()
    {
        Hide(false);
        GameManager.Instance.WindowService.ShowWindow<BoosterSelectWindow>(false);
    }

    private void UpdateReward(int reward)
    {
        coinText.text = "+ " + reward.ToString();
    }
}
