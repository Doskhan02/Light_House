using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GamePlayWindow : Window
{
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private TMP_Text timeText;
    [SerializeField] private Button gameStartButton;
    [SerializeField] private Button effectOnButton;
    [SerializeField] private Button effectOffButton;
    [SerializeField] private TMP_Text money;
    [SerializeField] private GameObject[] upgrades;

    public override void Initialize()
    {
        base.Initialize();
        scoreText.gameObject.SetActive(true);
        timeText.gameObject.SetActive(true);
        money.gameObject.SetActive(false);
        gameStartButton.gameObject.SetActive(false);
        effectOnButton.gameObject.SetActive(true);
        effectOffButton.gameObject.SetActive(true);
        for (int i = 0; i < upgrades.Length; i++) 
        {
            upgrades[i].SetActive(false);
        }
    }

    void Start()
    {
        money.gameObject.SetActive(true);
        scoreText.gameObject.SetActive(false);
        timeText.gameObject.SetActive(false);
        gameStartButton.gameObject.SetActive(true);
        effectOnButton.gameObject.SetActive(false);
        effectOffButton.gameObject.SetActive(false);
        for (int i = 0; i < upgrades.Length; i++)
        {
            upgrades[i].SetActive(true);
        }
    }


    void Update()
    {
        money.text = CurrencySystem.Instance.GetCurrency().ToString();

        scoreText.text = GameManager.Instance.ScoreSystem.Score + " / " + GameManager.Instance.gameData.targetScore;

        if (GameManager.Instance.SessionTimeInSeconds<10)
            timeText.text = GameManager.Instance.SessionTimeInMinutes + " : 0" + GameManager.Instance.SessionTimeInSeconds;
        else
            timeText.text = GameManager.Instance.SessionTimeInMinutes + " : " + GameManager.Instance.SessionTimeInSeconds;
    }
}
