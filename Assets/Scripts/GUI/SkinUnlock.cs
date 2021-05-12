using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkinUnlock : MonoBehaviour
{
    [SerializeField] Button keepBtn;
    [SerializeField] Button looseBtn;
    [SerializeField] Text scoreCoinText;
    [SerializeField] List<GameObject> skins;
  
    public Action keepIt;

    private void OnEnable()
    {
        keepBtn.onClick.AddListener(OnContinue);
        looseBtn.onClick.AddListener(OnCancel);
    }

    private void OnDisable()
    {
        keepBtn.onClick.RemoveListener(OnContinue);
        looseBtn.onClick.RemoveListener(OnCancel);
    }

    private void OnContinue()
    {
        keepIt?.Invoke();
    }

    private void OnCancel()
    {
       
    }

    public void Initialize(int totalScore, int coinScore)
    {
        scoreCoinText.text = coinScore.ToString();
        skins[GamePlayerPrefs.SkinIndex].SetActive(true);
    }
}
