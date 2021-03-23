using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;

public class SliderClampLabels : MonoBehaviour
{
    [SerializeField] Slider slider;
    [SerializeField] RangeSlider rangeSlider;
    [SerializeField] Text valueLabel;
    [SerializeField] Text minLabel;
    [SerializeField] Text maxLabel;

    private void OnEnable()
    {
        if (slider)
        {
            minLabel.text = $"{slider.minValue}";
            maxLabel.text = $"{slider.maxValue}";
            slider.onValueChanged.AddListener(onValueChanged);
            onValueChanged(slider.value);
        }
        else if (rangeSlider)
        {
            rangeSlider.OnValueChanged.AddListener(onValueChanged);
            onValueChanged(rangeSlider.LowValue, rangeSlider.HighValue);

        }
    }

    private void OnDisable()
    {
        if (slider)
            slider.onValueChanged.RemoveListener(onValueChanged);
        if (rangeSlider)
            rangeSlider.OnValueChanged.RemoveListener(onValueChanged);
    }

    private void onValueChanged(float value)
    {
        valueLabel.text = $"{value}";
    }

    private void onValueChanged(float min, float max)
    {
        minLabel.text = $"{min:P0}";
        maxLabel.text = $"{max:P0}";
    }
}
