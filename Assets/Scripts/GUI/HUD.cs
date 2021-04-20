using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class HUD : MonoBehaviour
{
    public AdditionalTime additional;

    [SerializeField] GameObject normalGamePlayPnl;
    [SerializeField] GameObject endlessGamePlayPnl;
    [SerializeField] GameObject timerContainer;
    [SerializeField] GameObject arenaGamePlayPnl;

    [SerializeField] Image lineImg;

    [SerializeField] Button pauseBtn;
    

    [SerializeField] Text timeText;
    [SerializeField] Text levelText;
    [SerializeField] Text scoreText;
    [SerializeField] Text pointsText;
    [SerializeField] Text arenaTimerText;

    public event UnityAction OnPause
    {
        add => pauseBtn.onClick.AddListener(value);
        remove => pauseBtn.onClick.RemoveListener(value);
    }

    public void SetActiveNormalPanel()
    {
        normalGamePlayPnl.SetActive(true);
        timerContainer.SetActive(true);
    }

    public void SetEndlessPanel()
    {
        endlessGamePlayPnl.SetActive(true);
        timerContainer.SetActive(true);
    }

    public void SetArenaPanel()
    {
        arenaGamePlayPnl.SetActive(true);
        timerContainer.SetActive(false);
        normalGamePlayPnl.SetActive(false);
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

    public void PointsAmount(int value)
    {
        pointsText.text = $"{value}";
    }

    public void UpDateCrashTimer(float _value, float time)
    {
        lineImg.fillAmount = _value;
        arenaTimerText.text = $"{(int) time}";
    }

}
