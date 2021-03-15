using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Controller : MonoBehaviour
{
    private static Controller instance;
    public static Controller Instance => instance;

    [SerializeField] private HUD hud;

    [SerializeField] private CameraController perspCamera;
    [SerializeField] private CameraController ortoCamera;

    [SerializeField] private Map mapPrefab;
    [SerializeField] private Player playerPrefab;

    [SerializeField] private LevelParameters level;

    public event Action Win;
    public event Action Loose;

    public GameParameters gameParameters;

    private CameraController camera;
    private Map map;
    private Player player;
    private float gameTimeLeft;

    public GameState gameState = GameState.Stop;

    public void StartGame(GameParameters parameters)
    {
        gameParameters = parameters;

        gameTimeLeft = gameParameters.duration;
        gameState = GameState.Play;


        if (gameParameters.isCameraOrthographic)
        {
            perspCamera.gameObject.SetActive(false);
            ortoCamera.gameObject.SetActive(true);
            camera = ortoCamera;
        }
        else
        {
            ortoCamera.gameObject.SetActive(false);
            perspCamera.gameObject.SetActive(true);
            camera = perspCamera;
        }

        level = new LevelParameters(gameParameters.areaFactor);

        map = Instantiate(mapPrefab, Vector3.zero, Quaternion.identity);
        map.Initializie(level);

        PlayerInit(level);
    }
    public void PlayerInit(LevelParameters level)
    {
        int xHeight = level.XHeight;
        float xOffset = level.XOffset;

        float playerX = xHeight * xOffset / 2;
        player = Instantiate(playerPrefab, new Vector3(playerX, 0.03f, 2.5f), Quaternion.identity);
        camera.player = player.transform;
    }

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);
    }

    private void Update()
    {
        if (gameState != GameState.Play)
            return;

        gameTimeLeft -= Time.deltaTime;

        if (gameTimeLeft <= 0)
        {
            Lost();
            gameTimeLeft = 0;
        }

        hud.UpdateScoreValue(gameTimeLeft);
    }

    public void Victory()
    {
        gameState = GameState.GameOver;

        Win?.Invoke();
        player.Win();
    }
    public void Lost()
    {
        gameState = GameState.GameOver;

        Loose?.Invoke();
        player.Loose();
    }
}
