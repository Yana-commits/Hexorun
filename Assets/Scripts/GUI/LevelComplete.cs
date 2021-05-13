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

        skins[GamePlayerPrefs.SkinIndex + 1].gameObject.SetActive(true);
        stroks[GamePlayerPrefs.SkinIndex + 1].gameObject.SetActive(true);
        Debug.Log($"{GamePlayerPrefs.SkinIndex}");
        skins[GamePlayerPrefs.SkinIndex + 1].fillAmount = (totalScore % 100)* 0.01f;
        pecentText.text = $"{totalScore%100}%";
    }
}
