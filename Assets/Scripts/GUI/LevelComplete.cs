using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class ResultPoPup : MonoBehaviour
{
    [SerializeField] private Button continuieBtn;
    [SerializeField] Text coinScoreText;
    [SerializeField] Text totalCoinsText;
    [SerializeField] private Image characterImg;
    [SerializeField] private Image strokeImg;
    [SerializeField] private Text percentText;
    [SerializeField] List<Sprite> skins;
    [SerializeField] List<Sprite> stroks;

    public Action continuePlay;

    public virtual void Initialize(int totalCoins, int currentCoins, int bestScore, int currentScore)
    {
        totalCoinsText.text = totalCoins.ToString();
        coinScoreText.text = currentCoins.ToString();
        SetSkin(GamePlayerPrefs.SkinIndex + 1);
        characterImg.gameObject.SetActive(true);
        strokeImg.gameObject.SetActive(true);

        float currentView =  ((float)totalCoins / 50);
        characterImg.fillAmount = ((float)(totalCoins - currentCoins) / 50);
        DOTween.To(() => characterImg.fillAmount, x => characterImg.fillAmount = x, currentView, 1);

        int currentPercent = ((totalCoins - currentCoins) / 50) * 100;

        DOTween.To(() => currentPercent, x => currentPercent = x,(int)(currentView * 100), 1)
            .OnUpdate(() => {
                percentText.text = currentPercent <= 100 ? currentPercent.ToString() + "%" : "100%";
            })
            .OnComplete(() => {
                continuieBtn.gameObject.SetActive(true);
            });
    }

    private void OnEnable()
    {
        continuieBtn.onClick.AddListener(OnContinue);

    }

    private void OnDisable()
    {
        continuieBtn.onClick.RemoveListener(OnContinue);
    }

    private void OnContinue()
    {
        continuePlay?.Invoke();
    }

    private void SetSkin(int id)
    {
        if (id < 0 || id >= skins.Count)
        {
            return;
        }
        characterImg.sprite = skins[id];
        strokeImg.sprite = stroks[id];
    }
}

public class LevelComplete : ResultPoPup
{

    public override void Initialize(int totalCoins, int currentCoins, int bestScore, int currentScore)
    {
        base.Initialize(totalCoins, currentCoins, bestScore, currentScore);
       
    }
}
