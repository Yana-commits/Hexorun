using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NormalMode : Mode
{
    private Player player;
    private HUD hud;
    private GameParameters gameParameters;
    private Chunk chunk;
    private Chunk ch;

    private float elapsedTime;
    private float duration;


    private float additionalTimePanel = 6;
    private float additionalTime = 10;
    private bool IsAdditionalTime = false;
    private float generatorTime;
    float hexRadius = 0.9755461f / 2;

    // Start is called before the first frame update
    void Start()
    {
        player.forPass += LoadPass;
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


        chunk = Instantiate(chunkPrefab,this.transform);
        chunk.Initialize(player.transform, new RectShape(),gameParameters);
        chunk.Map.SetTarget();
        ch = Instantiate(chunkPrefab, this.transform);
        Debug.Log($"{chunk.Map.targetIndex}");

        var hex = chunk.Map[new Vector2Int(gameParameters.size.x / 2, 0)];
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
        if (generatorTime > gameParameters.changesTime)
        {
            ChangedHexState(KindOfMapBehavor.DiffMoove);
            generatorTime = 0;
        }

        elapsedTime += Time.deltaTime;

        if (elapsedTime > duration)
        {
            elapsedTime = duration;
            gameState.SetGameState(GameplayState.GameOver);
            CheckForAdditionalTime();
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

    private void LoadPass()
    {

      var  nextChunkPos = chunk.Map.Bounds.size.z - hexRadius;
       ch.Map.Initializie(new Vector2Int(2,10), new RectShape(), gameParameters.theme);
        ch.Map.gameObject.SetActive(true);
        //var passX = Hexagonal.Offset.QToCube(chunk.Map.targetIndex);
        var passX = Hexagonal.Cube.HexToPixel(
               Hexagonal.Offset.QToCube(chunk.Map.targetIndex),
               Vector2.one * hexRadius);
        ch.transform.localPosition = new Vector3(passX.x, 0, nextChunkPos);
       
    }

    private void OnDisable()
    {
        player.forPass -= LoadPass;
    }
}
