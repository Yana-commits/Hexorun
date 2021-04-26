using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

    private List<Chunk> pass = new List<Chunk>();
    private List<Hex> visiblePass = new List<Hex>();


    private float additionalTimePanel = 6;
    private float additionalTime = 10;
    private bool IsAdditionalTime = false;
    private float generatorTime;
    float hexRadius = 0.9755461f / 2;

    
    private float depthFull=0;
    private float depthHalf= 0;
    private int currentChunkIndex;
    private float nextChunkPos = 0;
    private Vector3 passX;
    private bool firstChunk = true;

    // Start is called before the first frame update
    void Start()
    {
        player.forPass += LoadPass1;
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

        if (player.transform.position.z >= depthHalf + chunk.Map.Bounds.size.z)
        {
            LoadNextChunk();
        }
        if (player.transform.position.z >= depthFull + chunk.Map.Bounds.size.z)
        {
            ChangeCurrentChunk();
        }
      
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

    //private void LoadPass()
    //{

    //  var  nextChunkPos = chunk.Map.Bounds.size.z - hexRadius;
    //    ch = Instantiate(chunkPrefab, this.transform);
    //    ch.Map.Initializie(new Vector2Int(2,10), new RectShape(), gameParameters.theme);
    //    ch.Map.gameObject.SetActive(true);
    //    //var passX = Hexagonal.Offset.QToCube(chunk.Map.targetIndex);
    //    var passX = Hexagonal.Cube.HexToPixel(
    //           Hexagonal.Offset.QToCube(chunk.Map.targetIndex),
    //           Vector2.one * hexRadius);
    //    ch.transform.localPosition = new Vector3(passX.x, 0, nextChunkPos);
    //}

    private void LoadPass1()
    {
        var nextChunkPos = chunk.Map.Bounds.size.z - hexRadius ;

        passX = Hexagonal.Cube.HexToPixel(
              Hexagonal.Offset.QToCube(chunk.Map.targetIndex),
              Vector2.one * hexRadius);

        for (int i = 0; i < 10; i++)
        {
            ch = Instantiate(chunkPrefab, this.transform);
            ch.Map.Initializie(new Vector2Int(2, 10), new RectShape(), gameParameters.theme);
            ch.Map.gameObject.SetActive(true);

            ch.transform.localPosition = new Vector3(passX.x, 0, nextChunkPos);
      
            pass.Add(ch);
            ch.gameObject.SetActive(false);

            nextChunkPos += ch.Map.Bounds.size.z - hexRadius;
        }


        currentChunkIndex = 0;
        pass[currentChunkIndex].gameObject.SetActive(true);
        depthFull =  pass[currentChunkIndex].Map.Bounds.size.z;
        depthHalf =  depthFull / 2;
        visiblePass = pass[currentChunkIndex].Map.ToList();
     
       var bound = new Bounds(new Vector3(passX.x+0.5f, 0, chunk.Map.Bounds.size.z  + 5), new Vector3(1.5f, 0, chunk.Map.Bounds.size.z *10));
        //bound = pass[0].Map.Bounds;
        //bound.Expand(new Vector3(2, 0, 12));

        player.SetGamePlaySettings(gameParameters.playerSpeed+1, bound);
    }

    private void LoadNextChunk()
    {
       
        if (firstChunk)
        {
            nextChunkPos += chunk.Map.Bounds.size.z + ch.Map.Bounds.size.z - hexRadius * 2;
            firstChunk = false;
        }
        else 
        {
            nextChunkPos += ch.Map.Bounds.size.z - hexRadius ;
        }

        var chunkPass = pass[CheckNextIndex()];
      
        chunkPass.transform.localPosition = new Vector3(passX.x, 0, nextChunkPos);
        chunkPass.Map.SetTheme(gameParameters.theme);
        chunkPass.gameObject.SetActive(true);
        depthHalf = depthHalf + chunkPass.Map.Bounds.size.z -1;
        visiblePass.AddRange(chunkPass.Map.ToList());

       //var bound = chunkPass.Map.Bounds;
        //bound.Expand(new Vector3(0, 0, nextChunkPos));
        //bound.Encapsulate(new Vector3(passX.x, 0, nextChunkPos+10));
    }

    private int CheckNextIndex()
    {
        return currentChunkIndex +1  >= pass.Count ? 0 : currentChunkIndex + 1;
    }

    private void ChangeCurrentChunk()
    {
        currentChunkIndex = CheckNextIndex();
        depthFull += pass[currentChunkIndex].Map.Bounds.size.z -1;

        //foreach (var item in pass.GetRange(0, currentChunkIndex + 1))
        //{
        //    item.ChangeChunkTheme(gameParameters.theme);
        //}
    }

    private void OnDisable()
    {
        player.forPass -= LoadPass1;
    }
}
