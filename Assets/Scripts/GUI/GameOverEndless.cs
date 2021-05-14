using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameOverEndless : ResultPoPup
{
    [SerializeField] Text scoreText;
    [SerializeField] Text bestScoreText;
    public override void Initialize(int totalCoins, int currentCoins, int bestScore, int currentScore)
    {
        base.Initialize(totalCoins, currentCoins, bestScore, currentScore);
        bestScoreText.text = $"BEST : {bestScore}";
        scoreText.text = $"SCORE : {currentScore}";
    }
}
