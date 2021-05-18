using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Game.Main;
using UnityEngine.SceneManagement;

using Random = UnityEngine.Random;
using DG.Tweening;

public class GameState : MonoBehaviour
{
    [SerializeField] private HUD hud;


    [SerializeField] private Joystick joystick;

    [SerializeField] private EndlessMode endlessMode;
    [SerializeField] private NormalMode normalMode;
    [SerializeField] private ArenaMode arenaMode;

    [SerializeField] private MaterialRepository datas;
    [SerializeField] private LevelRepository levels;
    [SerializeField] private ModesRepository modesRepository;
    [SerializeField] private SkinRepository skinRepository;

    [Space]
    [SerializeField] private Player player;
    [SerializeField] private PlayerSkinController playerSkin;

    private GameParameters gameParameters;
    private Mode mode;

    private GameplayState gamePlayState = GameplayState.Stop;
    [Space]
    private GameModeState gameMode = GameModeState.Normal;

    private int _coinsCollect = 0;
    private int lastSkin = -1;
    public int CoinAmount
    {
        get => _coinsCollect;
        set
        {
            _coinsCollect = value;
            hud.ScoreAmount(_coinsCollect);
        }
    }

    private int _pointsCollect = 0;
    public int PointsAmount
    {
        get => _pointsCollect;
        set
        {
            _pointsCollect = value;
            hud.PointsAmount(_pointsCollect);
        }
    }

    public GameplayState GamePlayState { get => gamePlayState; }
    public MaterialRepository Datas { get => datas; set => datas = value; }
    public LevelRepository Levels { get => levels; set => levels = value; }

    public Action OnChangedHexPosition;

    private void Start()
    {
        hud.OnPause += () => { SetGameState(gamePlayState == GameplayState.Play ? GameplayState.Pause : GameplayState.Play); };
        Time.timeScale = 1;
        Application.targetFrameRate = 60;
        StartGame();
    }

    public void StartGame()
    {
        int level = Mathf.Min(GamePlayerPrefs.LastLevel + 1, modesRepository.Count - 1);
        lastSkin = GamePlayerPrefs.SkinIndex;

        gameParameters = modesRepository[level].gameParams;
        gameParameters.id = level;
        gameParameters.theme = datas.Materials[GamePlayerPrefs.LastTheme];

        gameMode = modesRepository[level].mode;
        mode = GetMode(gameMode);
        mode.gameParameters = gameParameters;
        mode.gameObject.SetActive(true);
        mode.Initialized(player, hud);
 
        PlayerInit();

        hud.UpdateLevel(level + 1);
        CoinAmount = 0;
        GamePlayerPrefs.LastTheme = (GamePlayerPrefs.LastTheme + 1) % datas.Count;
    }

    public void StartGameMode()
    {
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
        player.Initializie(joystick, gameParameters);
        player.passKlue = modesRepository[gameParameters.id].mode == GameModeState.NormalWithBonus ? true : false;
        player.OnStateChanged += OnPlayerStateChanged;
        player.enabled = false;
        playerSkin.Init(player.GetComponentInChildren<Animator>());
    }

    public void OnPlayerStateChanged(PlayerState obj)
    {
        SetGameState(GameplayState.GameOver);
        GamePlayerPrefs.LastGameMode = (int)gameMode;
        player.OnStateChanged -= OnPlayerStateChanged;

        switch (obj)
        {
            case PlayerState.Win:
                CountParams();
                StartCoroutine(player.Winner(Complete));
                break;
            case PlayerState.BigWin:
                StartCoroutine(player.BigWinner(Complete));
                CountParams();
                Debug.Log("999");
                break;
            case PlayerState.Lose:
                StartCoroutine(player.Looser(ReloadScene));
                break;
            case PlayerState.Fall:
                DOTween.KillAll();
                KindOfFall();
                break;
            case PlayerState.BonusFall:
                FallWithCoins();
                break;
            default:
                break;
        }
    }

    private void KindOfFall()
    {
        switch (gameMode)
        {
            case GameModeState.Endless:
                
                EndlessPlayerFall();
                break;
            case GameModeState.Arena:
                AfterFall();
                break;
            case GameModeState.Normal:
                 AfterFall();
                break;
            case GameModeState.NormalWithBonus:
                FallWithCoins();
                break;
        }
    }

    private void FallWithCoins()
    {
        CountParams();
        StartCoroutine(player.PassFall(Complete));
    }

    private void CountParams()
    {
        GamePlayerPrefs.LastLevel = gameParameters.id;
        GamePlayerPrefs.TotalCoins += CoinAmount;
    }

    private void Complete()
    {
        hud.gamePlay.SetActive(false);
        hud.InitResultPopUp(gameMode);
        hud.ShowResultPopUp(GamePlayerPrefs.TotalCoins, _coinsCollect, GamePlayerPrefs.BestScore, PointsAmount,ReloadScene, skinRepository);
    }
    private void EndlessPlayerFall()
    {
        CountParams();
        CheckBestScore();
        player.rigidbody.useGravity = false;
        Complete();
    }

    private void AfterFall()
    {
        hud.gamePlay.SetActive(true);
        StartCoroutine(player.Reload(ReloadScene));
    }

    private void ReloadScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void OnDestroy()
    {
        if (player)
            player.OnStateChanged -= OnPlayerStateChanged;
    }


    private void CheckBestScore()
    {
        if (gameMode == GameModeState.Endless && PointsAmount > GamePlayerPrefs.BestScore)
            GamePlayerPrefs.BestScore = PointsAmount;
    }

    private void LoseSkin()
    {
      GamePlayerPrefs.SkinIndex = lastSkin;
        ReloadScene();
    }

    private Mode GetMode(GameModeState gameState)
    {
        switch (gameState)
        {
            case GameModeState.Normal:
                return normalMode;
            case GameModeState.Endless:
                return endlessMode;
            case GameModeState.Arena:
                return arenaMode;
            case GameModeState.NormalWithBonus:
                return normalMode;
            default:
                return normalMode;
        }
    }
}
public enum KindOfMapBehavor
{
    AllDown,
    DiffMoove
}