using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GamePlayWindow : MonoBehaviour
{
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private TMP_Text timeText;
    [SerializeField] private Button gameStartButton;
    [SerializeField] private GameObject BG;

    public void Initialize()
    {
        scoreText.gameObject.SetActive(true);
        timeText.gameObject.SetActive(true);
        gameStartButton.gameObject.SetActive(false);
        BG.SetActive(false);
    }

    void Start()
    {
        scoreText.gameObject.SetActive(false);
        timeText.gameObject.SetActive(false);
        gameStartButton.gameObject.SetActive(true);
        BG.SetActive(true);
    }


    void Update()
    {
        
        scoreText.text = GameManager.Instance.ScoreSystem.Score + " / " + GameManager.Instance.gameData.targetScore;

        if (GameManager.Instance.SessionTimeInSeconds<10)
            timeText.text = GameManager.Instance.SessionTimeInMinutes + " : 0" + GameManager.Instance.SessionTimeInSeconds;
        else
            timeText.text = GameManager.Instance.SessionTimeInMinutes + " : " + GameManager.Instance.SessionTimeInSeconds;
    }
}
