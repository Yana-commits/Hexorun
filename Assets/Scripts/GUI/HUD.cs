using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUD : MonoBehaviour
{
    [SerializeField] Text timeText;
    [SerializeField] Text levelText;
    [SerializeField] Text scoreText;

    private int coinAmount = 0;

    private void OnEnable()
    {
        Star.Score += ScoreAmount;
    }
    public void UpdateScoreValue(float value)
    {
        timeText.text = TimeSpan.FromSeconds(value).ToString(@"mm\:ss");
    }

    public void UpdateLevel(int level)
    {
        levelText.text = $"Level {level}";
    }
    public void ScoreAmount()
    {
        coinAmount++;

        scoreText.text = coinAmount.ToString();
    }

    private void OnDisable()
    {
        Star.Score -= ScoreAmount;
    }
}
