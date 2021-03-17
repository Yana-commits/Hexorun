using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class StartGameWindow : MonoBehaviour
{
    public Slider playerSpeed;
    public Slider areaFactor;
    public Slider holes;
    public Button startButton;
    public int time;
        
    public StartGameEvent OnStartGame;
    private void OnStartClick()
    {

        OnStartGame?.Invoke(new GameParameters { 
            size = new Vector2Int(10, 20 +  (int)(areaFactor.value-1) * 6), // 20 * 30% = 6
            duration = time,
            playerSpeed = playerSpeed.value,
            changesTime = 2,
            holes = (int)holes.value,
        });
    }

    private void OnEnable() => startButton.onClick.AddListener(OnStartClick);
    private void OnDisable() => startButton.onClick.RemoveListener(OnStartClick);
}
