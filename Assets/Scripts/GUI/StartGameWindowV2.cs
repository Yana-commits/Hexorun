using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;

public class StartGameWindowV2 : MonoBehaviour
{
    [SerializeField] MaterialRepository datas;
    [SerializeField] LevelRepository levels;

    public Button startButton;
    public Dropdown themeDropDown;
        
    public StartGameEvent OnStartGame;
    private void OnStartClick()
    {
        GamePlayerPrefs.LastTheme = themeDropDown.value;
        int level = Mathf.Min(GamePlayerPrefs.LastLevel + 1, levels.Count-1);
        var gameParams = levels[level];
        gameParams.id = level;
        gameParams.theme = datas.Materials[themeDropDown.value];
        OnStartGame?.Invoke(gameParams);

        print(level);
    }

    private void OnEnable()
    {
        startButton.onClick.AddListener(OnStartClick);

        var names = datas
            .Select(d => new Dropdown.OptionData(d.name))
            .ToList();

        themeDropDown.ClearOptions();
        themeDropDown.AddOptions(names);
        themeDropDown.value = GamePlayerPrefs.LastTheme;
    }
    private void OnDisable() => startButton.onClick.RemoveListener(OnStartClick);
}
