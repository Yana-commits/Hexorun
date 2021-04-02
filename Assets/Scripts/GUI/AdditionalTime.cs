using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using DG.Tweening;

public class AdditionalTime : MonoBehaviour
{
    [SerializeField] Button continuieBtn;
    [SerializeField] Button giveUpBtn;
    [SerializeField] Image circleImg;
    [SerializeField] Text timerLabel;

    private float timer;
    public event UnityAction OnAddTime
    {
        add => continuieBtn.onClick.AddListener(value);
        remove => continuieBtn.onClick.RemoveListener(value);
    }
    public event UnityAction OnContiniue
    {
        add => giveUpBtn.onClick.AddListener(value);
        remove => giveUpBtn.onClick.RemoveListener(value);
    }

    public void SetTimer(float _timer)
    {
        timer = _timer;
        circleImg.DOFillAmount(0.0f, timer).OnUpdate(()=> {
            
            timerLabel.text = ((int)(circleImg.fillAmount * (timer)) + 1).ToString();
        });
    }

}
