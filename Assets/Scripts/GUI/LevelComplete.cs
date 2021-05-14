using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelComplete : MonoBehaviour
{
    [SerializeField] Button continuieBtn;
    [SerializeField] Button giveUpBtn;
    [SerializeField] Text scoreCoinText;
    [SerializeField] Text totalScoreText;
    private Image characterImg;
    [SerializeField] Text pecentText;
    [SerializeField] List<Image> skins;
    [SerializeField] List<Image> stroks;
    private int myFloat = 0;

    public Action continuePlay;
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
        continuePlay?.Invoke();
    }

    private void OnCancel()
    {
        continuePlay?.Invoke();
    }

    public void Initialize(int totalScore, int coinScore)
    {
        totalScoreText.text = totalScore.ToString();
        scoreCoinText.text = coinScore.ToString();

        characterImg = skins[1];
        characterImg.gameObject.SetActive(true);
        stroks[1].gameObject.SetActive(true);

        var currentView = (totalScore % 100) * 0.01f;
        characterImg.fillAmount = ((totalScore - coinScore) % 100) * 0.01f;
        DOTween.To(() => characterImg.fillAmount, x => characterImg.fillAmount = x, currentView, 1);

        myFloat = (totalScore - coinScore)%100;
        DOTween.To(() => myFloat, x => myFloat = x, totalScore, 1)
            .OnUpdate(()=> {
                pecentText.text = (myFloat%100).ToString() + "%";              
            })
            .OnComplete(()=> {
                continuieBtn.gameObject.SetActive(true);
            });       
    }

   
}
