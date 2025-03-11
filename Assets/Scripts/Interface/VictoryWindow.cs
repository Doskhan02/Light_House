using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class VictoryWindow : MonoBehaviour
{
    [SerializeField] TMP_Text scoreText;

    public void Start()
    {
        gameObject.SetActive(false);
    }
    public void Initialize(float score, int timeInSec, int timeInMin)
    {
        scoreText.text = "Your Score: " + (int)(score * 6000 * 1f/(timeInSec + timeInMin * 60));
        CurrencySystem.Instance.AddCurrency((int)(score * 6000 * 1f / (timeInSec + timeInMin * 60)));
    }
}
