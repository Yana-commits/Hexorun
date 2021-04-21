using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameOverEndless : MonoBehaviour
{
    [SerializeField] Button continuieBtn;
    [SerializeField] Button giveUpBtn;
    [SerializeField] Text scoreText;
    [SerializeField] Text bestScoreText;
    [SerializeField] Text coinsText;

    public Action continueFall;

    private void OnEnable()
    {
        continuieBtn.onClick.AddListener(OnContinue);
        giveUpBtn.onClick.AddListener(OnCancel);
    }

    private void OnDisable()
    {
        continuieBtn.onClick.RemoveListener(OnContinue);
        giveUpBtn.onClick.RemoveListener(OnCancel);
        continueFall = null;
    }

    private void OnContinue()
    {
        continueFall?.Invoke();
    }

    private void OnCancel()
    {
        continueFall?.Invoke();
    }

    public void Initialize(int score,int bestScore,int coinScore)
    {
        scoreText.text = $"SCORE : {score}";
        bestScoreText.text = $"BEST : {bestScore}";
        coinsText.text = coinScore.ToString();
    }
}
