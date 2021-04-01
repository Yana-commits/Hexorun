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
    [SerializeField] private AdditionalTime additional;
    [SerializeField] private Joystick joystick;

    [Space]
    [SerializeField] private Player player;
    [SerializeField] private Map map;
    [SerializeField] private ObstacleGenerator obstacleGenerator;
    [SerializeField] private ObstaclePresenter obstaclePresenter;
    [SerializeField] private StartGameWindow startGameWindow;

    private GameParameters gameParameters;

    private float elapsedTime;
    private float generatorTime;
    private GameplayState gameState = GameplayState.Stop;
    private float additionalTimePanel = 10;
    private float additionalTime = 10;

    private void Start()
    {
        hud.OnPause += () => SetGameState(GameplayState.Pause);
        additional.OnAddTime += () => AddTime();
    }

    public void StartGame(GameParameters parameters)
    {
        gameParameters = parameters;

        map.Initializie(gameParameters.size, gameParameters.theme);
        map.gameObject.SetActive(true);

        PlayerInit();
        obstaclePresenter.Initialize();
        obstacleGenerator.Initialize(player.transform, gameParameters.obstaclesParam);

        hud.UpdateLevel(gameParameters.id + 1);

        generatorTime = gameParameters.changesTime - 1;

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

        //SetGameState(GameplayState.Play);
    }

    public void SetGameState(GameplayState state)
    {
        gameState = state;
        switch (state)
        {
            case GameplayState.Stop:
                break;
            case GameplayState.Play:
                Time.timeScale = 1;
                player.enabled = true;
                break;
            case GameplayState.Pause:
                Time.timeScale = 0;
                player.enabled = false;
                break;
            case GameplayState.GameOver:
                DG.Tweening.DOTween.KillAll();
                player.enabled = false;
                break;
        }
    }

    private void PlayerInit()
    {
        var hex = map[gameParameters.size.x/2, 0];
        Vector3 startPos = hex.transform.position;
        player.transform.SetPositionAndRotation(startPos,Quaternion.identity);
        player.Initializie(gameParameters.playerSpeed, map.Bounds, joystick);
        player.stateChanged += OnPlayerStateChanged;
        player.enabled = false;
        player.gameObject.SetActive(true);
    }

    private void OnPlayerStateChanged(PlayerState obj)
    {
        SetGameState(GameplayState.GameOver);

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
        
        elapsedTime += Time.deltaTime;

        if (elapsedTime > gameParameters.duration)
        {
            StartCoroutine(ForAdditionalTime());
        }

        generatorTime += Time.deltaTime;
        if (generatorTime > gameParameters.changesTime)
        {
            obstacleGenerator.Generate();
            generatorTime = 0;
        }

        hud.UpdateScoreValue(gameParameters.duration - elapsedTime);
    }

    private void Lose()
    {
        OnPlayerStateChanged(PlayerState.Lose);
        elapsedTime = gameParameters.duration;
    }

    private IEnumerator ForAdditionalTime()
    {
        SetGameState(GameplayState.Pause);
        additional.gameObject.SetActive(true);

        yield return new WaitForSecondsRealtime(additionalTimePanel);

        SetGameState(GameplayState.Play);
        additional.gameObject.SetActive(false);
        
        Lose();
    }

    public void AddTime()
    {
        Debug.Log("111");

        StopCoroutine(ForAdditionalTime());
        gameParameters.duration = gameParameters.duration + (int)additionalTime;

        SetGameState(GameplayState.Play);
        additional.gameObject.SetActive(false);
    }
}
