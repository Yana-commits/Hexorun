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

    private GameParameters gameParameters;

    private float elapsedTime;
    private float generatorTime;
    private GameplayState gameState = GameplayState.Stop;

    private int _coinsCollect = 0;
    public int CoinAmount {
        get => _coinsCollect;
        set {
            _coinsCollect = value;
            hud.ScoreAmount(_coinsCollect);
        }
    }

    private float additionalTimePanel = 6;
    private float additionalTime = 10;
    private float duration;

    private bool IsAdditionalTime = false;

    private void Start()
    {
        hud.OnPause += () => { SetGameState(gameState == GameplayState.Play ? GameplayState.Pause : GameplayState.Play); };
    }

    public void StartGame(GameParameters parameters)
    {
        gameParameters = parameters;
        duration = gameParameters.duration;

        map.Initializie(gameParameters.size, gameParameters.theme);
        map.gameObject.SetActive(true);

        PlayerInit();
        obstaclePresenter.Initialize();
        obstacleGenerator.Initialize(player.transform, gameParameters.obstaclesParam);

        hud.UpdateLevel(gameParameters.id + 1);

        generatorTime = gameParameters.changesTime - 1;
        CoinAmount = 0;

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

    public void SetGameState(GameplayState state)
    {
        gameState = state;
        switch (state)
        {
            case GameplayState.Play:
                //Time.timeScale = 1;
                player.enabled = true;
                break;
            case GameplayState.Stop:
            case GameplayState.Pause:
            case GameplayState.GameOver:
                player.enabled = false;
                break;
        }
    }

    private void PlayerInit()
    {
        var hex = map[gameParameters.size.x / 2, 0];
        Vector3 startPos = hex.transform.position;
        player.transform.SetPositionAndRotation(startPos, Quaternion.identity);
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
                GamePlayerPrefs.TotalCoins += CoinAmount;
                break;
            case PlayerState.Lose:

                if (!IsAdditionalTime)
                {
                    player.speed = 0;
                    IsAdditionalTime = true;
                    additional.Initialize(additionalTimePanel, additionalTime, state => {
                        player.enabled = false;
                        if (state)
                        {
                            duration += additionalTime;
                            SetGameState(GameplayState.Play);
                            player.speed = gameParameters.playerSpeed;
                            player.enabled = true;
                        }
                        else
                        {
                            OnPlayerStateChanged(PlayerState.Lose);
                            player.speed = gameParameters.playerSpeed;
                            player.enabled = true;
                        }
                    });
                }
                else
                {
                    StartCoroutine(player.Looser(ReloadScene));
                }
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

        if (elapsedTime > duration)
        {
            elapsedTime = duration;
            OnPlayerStateChanged(PlayerState.Lose);
        }

        generatorTime += Time.deltaTime;
        if (generatorTime > gameParameters.changesTime)
        {
            obstacleGenerator.Generate();
            generatorTime = 0;
        }

        hud.UpdateScoreValue(duration - elapsedTime);
    }
}
