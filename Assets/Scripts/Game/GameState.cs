using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

using Random = UnityEngine.Random;

public class GameState : MonoBehaviour
{
    [SerializeField] private HUD hud;
    [SerializeField] private Joystick joystick;

    [Space]
    [SerializeField] private Player player;
    [SerializeField] private Map map;
    [SerializeField] private ObstacleGenerator obstacleGenerator;
    [SerializeField] private ObstaclePresenter obstaclePresenter;
    [SerializeField] private StartGameWindow startGameWindow;

    private GameParameters gameParameters;
    private float gameTimeLeft;
    private GameplayState gameState = GameplayState.Stop;

    public void StartGame(GameParameters parameters)
    {
     
        gameParameters = parameters;

        gameTimeLeft = gameParameters.duration;

        map.Initializie(gameParameters.size, gameParameters.theme);
        map.gameObject.SetActive(true);
        
        PlayerInit();
        obstaclePresenter.Initialize();
        obstacleGenerator.Initialize(player.transform, gameParameters.obstaclesParam);

        gameState = GameplayState.Play;
        StartCoroutine(ObstacleGeneratorLoop());

        hud.UpdateLevel(gameParameters.id + 1);

        //TODO: move into another place
        var list = map.Shuffle().ToList();
        int index = 0;

        foreach (var pair in gameParameters.collectableItems)
        {
            for (int i = 0; i < pair.Value; index++, i++)
            {
                GameObject star = Instantiate(pair.Key, list[index].transform);
                star.transform.localPosition = Vector3.up * 0.5f;
            }
        }

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
        DG.Tweening.DOTween.KillAll();
        player.enabled = false;

        switch (obj)
        {
            case PlayerState.Win:
                StartCoroutine(player.Winner(ReloadScene));
                GamePlayerPrefs.LastLevel = gameParameters.id;
                break;
            case PlayerState.Lose:
                StartCoroutine(player.Looser(ReloadScene));
                break;
            case PlayerState.Fall:
                StartCoroutine(player.FallDown(ReloadScene));
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
