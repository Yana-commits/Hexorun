using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Game.Main;
using UnityEngine.SceneManagement;

using Random = UnityEngine.Random;

public class GameState : MonoBehaviour
{
    [SerializeField] private HUD hud;

    [SerializeField] private Joystick joystick;

    [SerializeField] private EndlessMode endlessMode;
    [SerializeField] private NormalMode normalMode;
    [SerializeField] private ArenaMode arenaMode;

    [SerializeField] private PicGameMode picGameMode;

    [Space]
    [SerializeField] private Player player;
    [SerializeField] private PlayerSkinController playerSkin;

    private GameParameters gameParameters;
    private Mode mode;

    private GameplayState gamePlayState = GameplayState.Stop;
    [Space]
    [SerializeField]
    private GameModeState gameMode = GameModeState.Normal;

    private int _coinsCollect = 0;
    private int skinKoeff = 1;
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



    public Action OnChangedHexPosition;

    private void Start()
    {
        hud.OnPause += () => { SetGameState(gamePlayState == GameplayState.Play ? GameplayState.Pause : GameplayState.Play); };
        hud.overEndless.continueFall += ReloadScene;
        hud.levelComplete.continuePlay += ReloadScene;
        hud.skinUnlock.keepIt += ReloadScene;
        hud.skinUnlock.loseIt += LoseSkin;

        Time.timeScale = 1;
        Application.targetFrameRate = 60;
    }

    public void StartGame(GameParameters parameters)
    {
        gameParameters = parameters;
        lastSkin = GamePlayerPrefs.SkinIndex;

        mode = normalMode;
        mode.gameObject.SetActive(true);
        mode.Initialized(player, hud);

        PlayerInit();

        hud.UpdateLevel(gameParameters.id + 1);
        CoinAmount = 0;
    }

    public void StartGameMode()
    {
        gameMode = picGameMode.PicMode((GameModeState)GamePlayerPrefs.LastGameMode, GamePlayerPrefs.LastLevel);
      
        switch (gameMode)
        {
            case GameModeState.Normal:
                StartNormalMode();
                break;
            case GameModeState.Endless:
                StartEndlessMode();
                break;
            case GameModeState.Arena:
                StartArenaLevel();
                break;
            default:
                break;
        }
    }
    public void StartNormalMode()
    {
        SetGameState(GameplayState.Play);
        mode?.ChangedHexState(KindOfMapBehavor.DiffMoove);
    }

    public void StartEndlessMode()
    {
        gameMode = GameModeState.Endless;
        mode.gameObject.SetActive(false);
        mode = endlessMode;
        mode.gameObject.SetActive(true);
        mode.Initialized(player, hud);
        SetGameState(GameplayState.Play);
        mode?.ChangedHexState(KindOfMapBehavor.DiffMoove);
    }

    public void StartArenaLevel()
    {
        gameMode = GameModeState.Arena;
        mode.gameObject.SetActive(false);
        mode = arenaMode;
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
        player.Initializie(joystick, gameParameters);
        player.stateChanged += OnPlayerStateChanged;
        player.enabled = false;
        playerSkin.Init(player.GetComponentInChildren<Animator>());
    }

    public void OnPlayerStateChanged(PlayerState obj)
    {
        SetGameState(GameplayState.GameOver);
        GamePlayerPrefs.LastGameMode = (int)gameMode;

        switch (obj)
        {
            case PlayerState.Win:
                CountParams();
                StartCoroutine(player.Winner(Complete));
                break;
            case PlayerState.BigWin:
                CountParams();
                StartCoroutine(player.BigWinner(Complete));
                break;
            case PlayerState.Lose:
                StartCoroutine(player.Looser(ReloadScene));
                break;
            case PlayerState.Fall:
                player.stateChanged -= OnPlayerStateChanged;
                KindOfFall();
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
                CountParams();
                EndlessPlayerFall();
                break;
            case GameModeState.Arena:
                FallWithCoins();
                break;
            case GameModeState.Normal:
                if (player.passKlue == false)
                {
                    FallWithCoins();
                }
                else
                {
                    AfterFall();
                }
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
        if (gameMode == GameModeState.Normal)
        {
            GamePlayerPrefs.LastLevel = gameParameters.id;
        }
        GamePlayerPrefs.TotalCoins += CoinAmount;

        if (GamePlayerPrefs.TotalCoins >= 100*skinKoeff)
        {
            GamePlayerPrefs.SkinIndex = (int)(GamePlayerPrefs.TotalCoins/100) -1;
            skinKoeff++;
        }
    }

    private void Complete()
    {
        if (GamePlayerPrefs.TotalCoins >= 100 * skinKoeff)
        {
            hud.gamePlay.SetActive(false);
            hud.skinUnlock.gameObject.SetActive(true);
            hud.skinUnlock.Initialize(GamePlayerPrefs.TotalCoins);
        }
        else 
        {
            hud.gamePlay.SetActive(false);
            hud.levelComplete.gameObject.SetActive(true);
            hud.levelComplete.Initialize(GamePlayerPrefs.TotalCoins, _coinsCollect);
        }
    }
    private void EndlessPlayerFall()
    {
        CheckBestScore();
        Time.timeScale = 0;
        hud.gamePlay.SetActive(false);
        hud.overEndless.gameObject.SetActive(true);
        hud.overEndless.Initialize(PointsAmount, GamePlayerPrefs.BestScore, _coinsCollect, GamePlayerPrefs.TotalCoins);
    }

    private void AfterFall()
    {
        Time.timeScale = 1;
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
            player.stateChanged -= OnPlayerStateChanged;
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
}
public enum KindOfMapBehavor
{
    AllDown,
    DiffMoove
}