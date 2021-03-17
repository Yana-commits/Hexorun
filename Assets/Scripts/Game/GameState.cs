using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

using Random = UnityEngine.Random;

public class GameState : MonoBehaviour
{
    [SerializeField] private HUD hud;
    [SerializeField] private Joystick joystick;

    [SerializeField] private GameObject[] cameras;
    [Space]
    [SerializeField] private Player player;
    [SerializeField] private Map map;
    [SerializeField] private ObstacleGenerator obstacleGenerator;
    [SerializeField] private ObstaclePresenter obstaclePresenter;

    private GameParameters gameParameters;
    private float gameTimeLeft;
    private GameplayState gameState = GameplayState.Stop;

    public void StartGame(GameParameters parameters)
    {
        gameParameters = parameters;

        gameTimeLeft = gameParameters.duration;

        cameras[1].SetActive(gameParameters.isCameraOrthographic);
        cameras[0].SetActive(!gameParameters.isCameraOrthographic);

        map.Initializie(gameParameters.size);
        map.gameObject.SetActive(true);

        PlayerInit();
        obstaclePresenter.Initialize(gameParameters.holes);
        obstacleGenerator.Initialize(player.transform);

        gameState = GameplayState.Play;
        StartCoroutine(ObstacleGeneratorLoop());
    }

    private void PlayerInit()
    {
        var hex = map[gameParameters.size.x/2, 0];
        Vector3 startPos = hex.transform.position;
        player.transform.SetPositionAndRotation(startPos,Quaternion.identity);
        player.Initializie(gameParameters.playerSpeed, map.Bounds, joystick);
        player.stateChanged += OnPlayerStateChanged;
        player.gameObject.SetActive(true);
    }

    private void OnPlayerStateChanged(PlayerState obj)
    {
        gameState = GameplayState.GameOver;
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
        if (gameState != GameplayState.Play)
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
        while (gameState == GameplayState.Play)
        {
            obstacleGenerator.Generate();
            yield return new WaitForSeconds(gameParameters.changesTime);
        }
    }

}
