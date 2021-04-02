using UnityEngine;
using System.Collections;

public static class GamePlayerPrefs
{
    public static int LastTheme
    {
        get => PlayerPrefs.GetInt(nameof(LastTheme), 0);
        set => PlayerPrefs.SetInt(nameof(LastTheme), value);
    }

    public static int LastLevel
    {
        get => PlayerPrefs.GetInt(nameof(LastLevel), -1);
        set => PlayerPrefs.SetInt(nameof(LastLevel), value);
    }
    public static int BestScore
    {
        get => PlayerPrefs.GetInt(nameof(BestScore), 0);
        set => PlayerPrefs.SetInt(nameof(BestScore), value);
    }

}
