using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

using Random = UnityEngine.Random;

public class Controller : MonoBehaviour
{
    [SerializeField] private HUD hud;
    [SerializeField] private Joystick joystick;

    [SerializeField] private CameraController[] cameras;
    [Space]
    [SerializeField] private Map map;
    [SerializeField] private ObstacleGenerator obstacleGenerator;
    [SerializeField] private ObstaclePresenter obstaclePresenter;
    [Space]
    [SerializeField] private Player playerPrefab;

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
        var hex = map[gameParameters.size.x/2, 0];
        Vector3 startPos = hex.transform.position;
        player = Instantiate(playerPrefab, startPos,Quaternion.identity);
        player.Initializie(gameParameters.playerSpeed, map.Bounds, joystick);

        player.stateChanged += OnPlayerStateChanged;
    }

    private void OnPlayerStateChanged(PlayerState obj)
    {
        gameState = GameState.GameOver;
        player.enabled = false;

        switch (obj)
        {
            case PlayerState.Win:
                StartCoroutine(player.Winner(ReloadScene));
                break;
            case PlayerState.Lose:
                StartCoroutine(player.Looser(ReloadScene));
                break;
            case PlayerState.Fall:
                ReloadScene();
                break;
            default:
                break;
        }

    }

    private void ReloadScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void OnDestroy()
    {
        if (player)
            player.stateChanged -= OnPlayerStateChanged;
    }

    private void Update()
    {
        if (gameState != GameState.Play)
            return;

        gameTimeLeft -= Time.deltaTime;

        if (gameTimeLeft <= 0)
        {
            OnPlayerStateChanged(PlayerState.Lose);
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

}
