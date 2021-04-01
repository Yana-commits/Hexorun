using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class AdditionalTime : MonoBehaviour
{
    [SerializeField] Button continuieBtn;

    [SerializeField] Button giveUpBtn;

    public event UnityAction OnAddTime
    {
        add => continuieBtn.onClick.AddListener(value);
        remove => continuieBtn.onClick.RemoveListener(value);
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
