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

    [SerializeField] private EndlessMode endlessMode;
    [SerializeField] private NormalMode normalMode;

    [Space]
    [SerializeField] private Player player;

    private GameParameters gameParameters;
    private Mode mode;

    private float elapsedTime;
    private float generatorTime;
    private GameplayState gameState = GameplayState.Stop;
    [Space]
    [SerializeField]
    private GameModeState gameMode = GameModeState.Normal;

    private int _coinsCollect = 0;
    public int CoinAmount
    {
        get => _coinsCollect;
        set
        {
            _coinsCollect = value;
            hud.ScoreAmount(_coinsCollect);
        }
    }

    private float additionalTimePanel = 6;
    private float additionalTime = 10;
    private float duration;

    private bool IsAdditionalTime = false;

    public Action OnChangedHexPosition;

    private void Start()
    {
        hud.OnPause += () => { SetGameState(gameState == GameplayState.Play ? GameplayState.Pause : GameplayState.Play); };
    }

    public void StartGame(GameParameters parameters)
    {
        gameParameters = parameters;
        duration = gameParameters.duration;
       
        if (gameMode == GameModeState.Normal)
            mode = normalMode;
        else
            mode = endlessMode;
        mode.Initialized(player);
        PlayerInit();
        hud.UpdateLevel(gameParameters.id + 1);
        generatorTime = gameParameters.changesTime - 1;
        CoinAmount = 0;
    }

    public void StartNormalMode()
    {
        SetGameState(GameplayState.Play);
        mode?.ChangedHexState();
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
        player.Initializie(joystick);
        player.stateChanged += OnPlayerStateChanged;
        player.enabled = false;
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
                    IsAdditionalTime = true;
                    additional.Initialize(additionalTimePanel, additionalTime, state =>
                    {
                        if (state)
                        {
                            duration += additionalTime;
                            SetGameState(GameplayState.Play);
                        }
                        else
                        {
                            OnPlayerStateChanged(PlayerState.Lose);
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
            mode?.ChangedHexState();
            generatorTime = 0;
        }

        hud.UpdateScoreValue(duration - elapsedTime);
    }
}
