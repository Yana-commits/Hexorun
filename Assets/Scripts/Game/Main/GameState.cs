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

    [SerializeField] private EndlessMode endlessMode;
    [SerializeField] private NormalMode normalMode;

    [Space]
    [SerializeField] private Player player;

    private GameParameters gameParameters;
    private Mode mode;


    private float generatorTime ;
    private float generatorTimeForMap;
    private float changesTime =2;
    int n = 0;
    int m = 0;
    int stopDoDiffMoove = 3;
    int stopDoDown = 1;
    private GameplayState gamePlayState = GameplayState.Stop;
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

    public GameplayState GamePlayState { get => gamePlayState; }



    public Action OnChangedHexPosition;

    private void Start()
    {
        hud.OnPause += () => { SetGameState(gamePlayState == GameplayState.Play ? GameplayState.Pause : GameplayState.Play); };

    }

    public void StartGame(GameParameters parameters)
    {
        gameParameters = parameters;

        mode = normalMode;
        mode.gameObject.SetActive(true);
        mode.Initialized(player, hud);

        PlayerInit();

        hud.UpdateLevel(gameParameters.id + 1);
        generatorTime = gameParameters.changesTime - 1;

        CoinAmount = 0;
    }

    public void StartNormalMode()
    {
        SetGameState(GameplayState.Play);
        mode?.ChangedHexState(KindOfMapBehavor.DiffMoove);
    }

    public void StartEndlessMode()
    {
        mode.gameObject.SetActive(false);
        mode = endlessMode;
        mode.gameObject.SetActive(true);
        mode.Initialized(player, hud);
        SetGameState(GameplayState.Play);
        mode?.ChangedHexState(KindOfMapBehavor.DiffMoove);
    }

    public void SetGameState(GameplayState state)
    {
        gamePlayState = state;
        switch (state)
        {
            case GameplayState.Play:
                player.StartPlaying();
                break;
            case GameplayState.Stop:
            case GameplayState.Pause:
            case GameplayState.GameOver:
                player.StopPlayer();
                break;
        }
    }

    private void PlayerInit()
    {
        player.Initializie(joystick);
        player.stateChanged += OnPlayerStateChanged;
        player.enabled = false;
    }

    public void OnPlayerStateChanged(PlayerState obj)
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

    //private void Update()
    //{
    //    if (gamePlayState != GameplayState.Play)
    //        return;

    //    generatorTime += Time.deltaTime;
    //    if (generatorTime > changesTime)
    //    {
    //        changesTime = changesTime == 6 ? 2 : 6;

    //        mode?.ChangedHexState(KindOfMapBehavor.AllDown);

    //        generatorTime = 0;
    //    }
    //}

    private void Update()
    {
        if (gamePlayState != GameplayState.Play)
            return;

        generatorTime += Time.deltaTime;

        if ((generatorTime > gameParameters.changesTime) && (n <= stopDoDiffMoove))
        {
            mode?.ChangedHexState(KindOfMapBehavor.DiffMoove);
            n++;
            m = 0;
            generatorTime = 0;
        }
        else if ((generatorTime > gameParameters.changesTime) && (n > stopDoDiffMoove) && (m <= stopDoDown))
        {
            mode?.ChangedHexState(KindOfMapBehavor.AllDown);
            generatorTime = 0;
            m++;
        }
        else if (m > stopDoDown)
        {
            n = 0;
        }



    }

  
}
public enum KindOfMapBehavor
{
    AllDown,
    DiffMoove
}