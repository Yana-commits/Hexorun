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
    [SerializeField] GameState gameState;
    [SerializeField] Text levelText;

    public Button startButton;
  
    public StartGameEvent OnStartGame;

    private void Start()
    {
        int level = Mathf.Min(GamePlayerPrefs.LastLevel + 1, levels.Count - 1);
        var gameParams = levels[level];
        gameParams.id = level;
        gameParams.theme = datas.Materials[GamePlayerPrefs.LastTheme];
        GamePlayerPrefs.LastTheme = (GamePlayerPrefs.LastTheme + 1) % datas.Count;
        gameState.StartGame(gameParams);
        levelText.text = "Level " +(level + 1).ToString();
    }

    private void OnStartClick()
    {

        gameState.SetGameState(GameplayState.Play);
        OnStartGame?.Invoke(null);       
    }

    private void OnEnable()
    {
        startButton.onClick.AddListener(OnStartClick);

        var names = datas
            .Select(d => new Dropdown.OptionData(d.name))
            .ToList();
    }
    private void OnDisable() => startButton.onClick.RemoveListener(OnStartClick);
}
