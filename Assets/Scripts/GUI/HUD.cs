using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class HUD : MonoBehaviour
{
    public AdditionalTime additional;

    [SerializeField] Button pauseBtn;
    
    [SerializeField] Text timeText;
    [SerializeField] Text levelText;
    [SerializeField] Text scoreText;

    public event UnityAction OnPause
    {
        add => pauseBtn.onClick.AddListener(value);
        remove => pauseBtn.onClick.RemoveListener(value);
    }

    public void UpdateScoreValue(float value)
    {
        timeText.text = TimeSpan.FromSeconds(value).ToString(@"mm\:ss");
    }

    public void UpdateLevel(int level)
    {
        levelText.text = $"Level {level}";
    }

    public void ScoreAmount(int value)
    {
        scoreText.text = $"{value}";
    }
}
