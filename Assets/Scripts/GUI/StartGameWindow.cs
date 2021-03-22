using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class StartGameWindow : MonoBehaviour
{
    [SerializeField] MaterialRepository datas;

    public Slider playerSpeed;
    public Slider areaFactor;
    public Slider holes;
    public Button startButton;
    public Dropdown themeDropDown;
    public int time;
        
    public StartGameEvent OnStartGame;
    private void OnStartClick()
    {
        GamePlayerPrefs.LastTheme = themeDropDown.value;

        OnStartGame?.Invoke(new GameParameters
        {
            size = new Vector2Int(10, 20 + (int)(areaFactor.value - 1) * 6), // 20 * 30% = 6
            duration = time,
            playerSpeed = 2,
            changesTime = 2,
            holes = (int)(holes.value),
            theme = datas.Materials[themeDropDown.value]
        }) ;
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
