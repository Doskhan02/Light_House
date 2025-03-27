using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class VictoryWindow : MonoBehaviour
{
    [SerializeField] Button continueButton;
    [SerializeField] TMP_Text scoreText;
    [SerializeField] TMP_Text mainText;

    public void Start()
    {
        gameObject.SetActive(false);
        continueButton.interactable = false;
    }
    public void Initialize(float score, int timeInSec, int timeInMin, bool isVictory)
    {
        scoreText.text = "Your Score: " + (int)(score * 6000 * 1f/(timeInSec + timeInMin * 60));
        if (isVictory) 
        {
            continueButton.interactable = true;
            mainText.color = Color.green;
            mainText.text = "Victory!";
            CurrencySystem.Instance.AddCurrency((int)(score * 6000 * 1f / (timeInSec + timeInMin * 60)));
        }
        else
        {
            mainText.color = Color.red;
            mainText.text = "Defeat";
        }
        
    }
}
