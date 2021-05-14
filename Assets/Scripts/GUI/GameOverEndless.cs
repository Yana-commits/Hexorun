using DG.Tweening;
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
    [SerializeField] Image characterImg;
    [SerializeField] Text pecentText;
    [SerializeField] List<Image> skins;
    [SerializeField] List<Image> stroks;
    private int myScore = 0;

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
    }

    private void OnContinue()
    {
        continueFall?.Invoke();
    }

    private void OnCancel()
    {
        continueFall?.Invoke();
    }

    public void Initialize(int score, int bestScore, int coinScore, int totalCoins)
    {
        scoreText.text = $"SCORE : {score}";
        bestScoreText.text = $"BEST : {bestScore}";
        coinsText.text = coinScore.ToString();

        characterImg = skins[1];
        characterImg.gameObject.SetActive(true);
        stroks[1].gameObject.SetActive(true);

        var currentView = (totalCoins % 100) * 0.01f;
        characterImg.fillAmount = ((totalCoins - coinScore) % 100) * 0.01f;
        DOTween.To(() => characterImg.fillAmount, x => characterImg.fillAmount = x, currentView, 1);
          


        myScore = (totalCoins - coinScore);
        DOTween.To(() => myScore, x => myScore = x, totalCoins, 1)
            .OnUpdate(() => {
                pecentText.text = myScore.ToString() + "%";
            })
            .OnComplete(() => {
                continuieBtn.gameObject.SetActive(true);
            });
    }
}
