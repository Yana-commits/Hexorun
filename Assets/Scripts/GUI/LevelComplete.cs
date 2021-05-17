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

    public Action OnContinuePlay;
    public Action OnUnlockNewSkin;

    public virtual void Initialize(int totalCoins, int currentCoins, int bestScore, int currentScore)
    {
        totalCoinsText.text = totalCoins.ToString();
        coinScoreText.text = currentCoins.ToString();
        SetSkin(GamePlayerPrefs.SkinIndex + 1);
        int purposeCoins = 50 * GamePlayerPrefs.SkinKoeff;

        float currentView =  ((float)totalCoins % 100) / purposeCoins;
        float delta = (((float)(totalCoins - currentCoins) % 100) / purposeCoins);

        characterImg.fillAmount = delta;
        DOTween.To(() => characterImg.fillAmount, x => characterImg.fillAmount = x, currentView, 1);

        int currentPercent =  (int)(delta * 100);
        int totalPercent = (int)(currentView * 100) > 100 ? 100 : (int)(currentView * 100);

        DOTween.To(() => currentPercent, x => currentPercent = x, totalPercent, 1)
            .OnUpdate(() => {
                percentText.text = currentPercent > 100 ? "100%" : currentPercent.ToString() + "%";
            })
            .OnComplete(() => {
                continuieBtn.gameObject.SetActive(true);
                if (totalPercent >= 100)
                    OnUnlockNewSkin?.Invoke();

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
        OnContinuePlay?.Invoke();
    }

    private void SetSkin(int id)
    {
        id = id >= skins.Count ? 0 : id;
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
