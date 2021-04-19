using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArenaMode : Mode
{
    private Player player;
    private HUD hud;
    private GameParameters gameParameters;
    private Chunk chunk;

    private float elapsedTime;
    private float duration;

    private float additionalTimePanel = 6;
    private float additionalTime = 10;
    private bool IsAdditionalTime = false;
    private int m;
    private float generatorTime;
    private int n;
    private int stopDoDiffMoove = 3;
    private int stopDoDown = 1;

    // Start is called before the first frame update
    void Start()
    {
       
    }

    public override void Initialized(Player _player,HUD hud)
    {
        player = _player;
        this.hud = hud;
        hud.SetActiveNormalPanel();

        int level = Mathf.Min(GamePlayerPrefs.LastLevel + 1, levels.Count - 1);
        gameParameters = levels[level];
        gameParameters.id = level;
        gameParameters.theme = datas.Materials[GamePlayerPrefs.LastTheme];
        duration = gameParameters.duration;
        gameParameters.size = new Vector2Int(10, 40);

        chunk = Instantiate(chunkPrefab,this.transform);
        chunk.Initialize(player.transform, new HexShape(), gameParameters);
        chunk.GeneratePointItem();
        chunk.Map.SetArenaTarget();

        var hex = chunk.Map[new Vector2Int(0, -gameParameters.size.y +1)];
        Vector3 startPos = hex.transform.position;
        player.transform.SetPositionAndRotation(startPos, Quaternion.identity);
        player.SetGamePlaySettings(gameParameters.playerSpeed, chunk.Map.Bounds);
        player.gameObject.SetActive(true);
    }

    private void Update()
    {
        if (gameState.GamePlayState != GameplayState.Play)
            return;

        generatorTime += Time.deltaTime;

        if ((generatorTime > gameParameters.changesTime) && (n <= stopDoDiffMoove))
        {
            ChangedHexState(KindOfMapBehavor.DiffMoove);
            n++;
            m = 0;
            generatorTime = 0;
        }
        else if ((generatorTime > gameParameters.changesTime) && (n > stopDoDiffMoove) && (m <= stopDoDown))
        {
            ChangedHexState(KindOfMapBehavor.AllDown);
            generatorTime = 0;
            m++;
        }
        else if (m > stopDoDown)
        {
            n = 0;
        }
        hud.UpdateScoreValue(duration - elapsedTime);
    }

    public override void ChangedHexState(KindOfMapBehavor mapBehavor)
    {
        chunk.ChangeHexes(mapBehavor);
    }

    private void CheckForAdditionalTime()
    {
        if (!IsAdditionalTime)
        {
            player.StopPlayer();
            IsAdditionalTime = true;
           hud.additional.Initialize(additionalTimePanel, additionalTime, state =>
            {
                if (state)
                {
                    duration += additionalTime;
                    player.StartPlaying();
                    gameState.SetGameState(GameplayState.Play);
                }
                else
                {
                    gameState.OnPlayerStateChanged(PlayerState.Lose);
                }
            });
        }
        else
        {
            gameState.OnPlayerStateChanged(PlayerState.Lose);
        }
    }

}

