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

    public void Initialize(int score,int bestScore,int coinScore,int totalCoins)
    {
        scoreText.text = $"SCORE : {score}";
        bestScoreText.text = $"BEST : {bestScore}";
        coinsText.text = coinScore.ToString();

        skins[GamePlayerPrefs.SkinIndex + 1].gameObject.SetActive(true);
        stroks[GamePlayerPrefs.SkinIndex + 1].gameObject.SetActive(true);
        skins[GamePlayerPrefs.SkinIndex + 1].fillAmount = (totalCoins % 100) * 0.01f;
        pecentText.text = $"{totalCoins % 100}%";
    }
}
