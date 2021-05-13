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
    [SerializeField] Text totalCoins;
    [SerializeField] Text bestScore;

    public StartGameEvent OnStartGame;

    private void Start()
    {
        int level = GamePlayerPrefs.LastLevel + 1;

        levelText.text = "Level " + (level + 1).ToString();
        totalCoins.text = GamePlayerPrefs.TotalCoins.ToString();
        bestScore.text = "BEST: "+ GamePlayerPrefs.BestScore.ToString();
    }

    public void OnStartClick()
    {
        gameState.StartGameMode();
        OnStartGame?.Invoke(null);
    }
}
