using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using DG.Tweening;
using System;

public class AdditionalTime : MonoBehaviour
{
    [SerializeField] Button continuieBtn;
    [SerializeField] Button giveUpBtn;
    [SerializeField] Image circleImg;
    [SerializeField] Text timerLabel;
    [SerializeField] Text addTimeLbl;

    private float _duration = 6;
    private Action<bool> _callback;


    private void OnEnable()
    {
        continuieBtn.onClick.AddListener(OnContinue);
        giveUpBtn.onClick.AddListener(OnCancel);
    }

    private void OnDisable()
    {
        continuieBtn.onClick.RemoveListener(OnContinue);
        giveUpBtn.onClick.RemoveListener(OnCancel);
        _callback = null;
    }

    private void OnContinue()
    {
        _callback?.Invoke(true);
    }

    private void OnCancel()
    {
        _callback?.Invoke(false);
    }

    public void Initialize (float duration, float addSeconds, Action<bool> callback)
    {
        _duration = duration;
        _callback = callback;
        _callback += _ => gameObject.SetActive(false);

        addTimeLbl.text = $"{addSeconds:+#;-#;0}";

        StopCoroutine(nameof(Countdown));
        gameObject.SetActive(true);
        StartCoroutine(nameof(Countdown));
    }

    private IEnumerator Countdown()
    {
        circleImg.fillAmount = 1f;
        timerLabel.text = $"{_duration}";

        float elapsedTime = 0;
        while (elapsedTime <= _duration)
        {
            yield return null;

            elapsedTime += Time.deltaTime;
            float normalizedValue = Mathf.Clamp01(elapsedTime/_duration);
            circleImg.fillAmount = 1f - normalizedValue;

            timerLabel.text = $"{Mathf.CeilToInt(_duration - elapsedTime)}";
        }

        _callback?.Invoke(false);
    }

}
