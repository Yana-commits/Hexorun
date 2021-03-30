using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUD : MonoBehaviour
{
    [SerializeField] Text timeText;
    [SerializeField] Text levelText;

    public void UpdateScoreValue(float value)
    {
        timeText.text = TimeSpan.FromSeconds(value).ToString(@"mm\:ss");
    }

    public void UpdateLevel(int level)
    {
        levelText.text = $"Level {level}";
    }

}
