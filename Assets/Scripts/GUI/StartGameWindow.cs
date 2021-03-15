using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class StartGameWindow : MonoBehaviour
{
    public Slider timer;
    public Slider playerSpeed;
    public Slider changesTime;
    public Slider areaFactor;
    public Slider cameraFactor;
    public Slider holes;
    public Button startButton;

    public StartGameEvent OnStartGame;
    private void OnStartClick()
    {
        OnStartGame?.Invoke(new GameParameters { 
            duration = (int)timer.value,
            playerSpeed = playerSpeed.value,
            changesTime = changesTime.value,
            areaFactor = (int)areaFactor.value,
            holes = (int)holes.value,
            isCameraOrthographic = cameraFactor.value == 1
        });
    }

    private void OnEnable() => startButton.onClick.AddListener(OnStartClick);
    private void OnDisable() => startButton.onClick.RemoveListener(OnStartClick);
}
