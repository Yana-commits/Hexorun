using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUD : MonoBehaviour
{
    [SerializeField] Text timeText;

    public void UpdateScoreValue(float value)
    {
        timeText.text = Mathf.Round(value).ToString();
    }

}
