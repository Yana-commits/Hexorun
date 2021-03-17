using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SliderClampLabels : MonoBehaviour
{
    [SerializeField] Slider slider;
    [SerializeField] Text valueLabel;
    [SerializeField] Text minLabel;
    [SerializeField] Text maxLabel;

    private void Start()
    {
        minLabel.text = $"{slider.minValue}";
        maxLabel.text = $"{slider.maxValue}";
    }

    private void OnEnable()
    {
        slider.onValueChanged.AddListener(onValueChanged);
        onValueChanged(slider.value);
    }

    private void OnDisable()
    {
        slider.onValueChanged.RemoveListener(onValueChanged);
    }

    private void onValueChanged(float value)
    {
        valueLabel.text = $"{value}";
    }
}
