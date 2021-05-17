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
  
    public Action OnKeepIt;
    public Action OnLoseIt;

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
        OnKeepIt?.Invoke();
    }

    private void OnCancel()
    {
       OnLoseIt?.Invoke();
    }

    public void Initialize(int totalScore)
    {
        scoreCoinText.text = totalScore.ToString();
        if (GamePlayerPrefs.SkinIndex < skins.Count && GamePlayerPrefs.SkinIndex >= 0)
        skins[GamePlayerPrefs.SkinIndex].SetActive(true);

    }
}
