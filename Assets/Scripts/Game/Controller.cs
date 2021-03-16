using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

using Random = UnityEngine.Random;

public class Controller : MonoBehaviour
{
    private static Controller instance;
    public static Controller Instance => instance;

    [SerializeField] private HUD hud;

    [SerializeField] private CameraController[] cameras;
    [Space]
    [SerializeField] private Map map;
    [SerializeField] private ObstacleGenerator obstacleGenerator;
    [SerializeField] private ObstaclePresenter obstaclePresenter;
    [Space]
    [SerializeField] private Player playerPrefab;

    public event Action Win;
    public event Action Loose;

    public GameParameters gameParameters;

    private Player player;
    private float gameTimeLeft;

    public GameState gameState = GameState.Stop;

    public void StartGame(GameParameters parameters)
    {
        gameParameters = parameters;

        gameTimeLeft = gameParameters.duration;

        cameras[1].gameObject.SetActive(gameParameters.isCameraOrthographic);
        cameras[0].gameObject.SetActive(!gameParameters.isCameraOrthographic);

        map.Initializie(gameParameters.size);

        PlayerInit();
        obstaclePresenter.Initialize(gameParameters.holes);
        obstacleGenerator.Initialize(player.transform);

        foreach (var camera in cameras)
            camera.player = player.transform;

        gameState = GameState.Play;
        StartCoroutine(ObstacleGeneratorLoop());
    }

    public void PlayerInit()
    {
        var hex = map[Random.Range(0, gameParameters.size.x), 1];
        Vector3 startPos = hex.transform.position;
        startPos.y = 0.03f;
        player = Instantiate(playerPrefab, startPos,Quaternion.identity);
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

    private IEnumerator ObstacleGeneratorLoop()
    {
        yield return new WaitForSeconds(1f);
        while (gameState == GameState.Play)
        {
            obstacleGenerator.Generate();
            yield return new WaitForSeconds(gameParameters.changesTime);
        }
    }

    public void Victory()
    {
        //TODO: need refactoring
        gameState = GameState.GameOver;

        Win?.Invoke();
        player.Win();
    }

    public void Lost()
    {
        //TODO: need refactoring
        gameState = GameState.GameOver;

        Loose?.Invoke();
        player?.Loose();
    }
}
