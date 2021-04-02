using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class HUD : MonoBehaviour
{
    [SerializeField] Button pauseBtn;

    [SerializeField] Text timeText;
    [SerializeField] Text levelText;
    [SerializeField] Text scoreText;

    public int coinAmount = 0;

    public event UnityAction OnPause
    {
        add => pauseBtn.onClick.AddListener(value);
        remove => pauseBtn.onClick.RemoveListener(value);
    }

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
