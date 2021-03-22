using UnityEngine;
using System.Collections;

public static class GamePlayerPrefs
{
    public static int LastTheme {
        get => PlayerPrefs.GetInt(nameof(LastTheme), 0);
        set => PlayerPrefs.SetInt(nameof(LastTheme), value);
    }
}
