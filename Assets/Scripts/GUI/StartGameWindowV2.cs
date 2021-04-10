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
    [SerializeField] Text bestScore;
    [SerializeField] Button endlessBtn;

    public StartGameEvent OnStartGame;

    private void Start()
    {
        endlessBtn.onClick.AddListener(() =>
        {
            gameState?.StartEndlessMode();
            OnStartGame?.Invoke(null);
        });

        int level = Mathf.Min(GamePlayerPrefs.LastLevel + 1, levels.Count - 1);
        var gameParams = levels[level];
        gameParams.id = level;
        gameParams.theme = datas.Materials[GamePlayerPrefs.LastTheme];
        GamePlayerPrefs.LastTheme = (GamePlayerPrefs.LastTheme + 1) % datas.Count;
        gameState.StartGame(gameParams);
        levelText.text = "Level " + (level + 1).ToString();
        bestScore.text = GamePlayerPrefs.TotalCoins.ToString();
    }

    public void OnStartClick()
    {
        gameState.StartNormalMode();
        OnStartGame?.Invoke(null);
    }
}
