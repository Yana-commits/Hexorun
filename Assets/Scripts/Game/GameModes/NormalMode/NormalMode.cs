using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class NormalMode : Mode
{
    private Player player;
    private HUD hud;
    private Chunk chunk;
    private Chunk ch;
    private Chunk platform;

    private float elapsedTime;
    private float duration;

    private List<Chunk> pass = new List<Chunk>();
    private List<Hex> visiblePass = new List<Hex>();
    public List<Multiplyer> multItems = new List<Multiplyer>();
    private List<Multiplyer> visibleMult = new List<Multiplyer>();
    [SerializeField]
    GameObject thronePrefab;


    private float additionalTimePanel = 6;
    private float additionalTime = 10;
    private bool IsAdditionalTime = false;
    private float generatorTime;
    float hexRadius = 0.9755461f / 2;
    private float loadTime;
    private float loadMultTime;


    private int currentChunkIndex;
    private float nextChunkPos = 0;
    private Vector3 passX;
    private bool firstChunk = true;
    private bool isPass = true;
    private int chankCounter = 0;
    private bool isPlatform = true;
    private int j = 0;
    public float appearTime;

    // Start is called before the first frame update
    void Start()
    {
        player.OnPassActivated += LoadPass1;
    }

    public override void Initialized(Player _player,HUD hud)
    {
        player = _player;
        this.hud = hud;
        hud.SetActiveNormalPanel();

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

        if (isPass)
        {
            elapsedTime += Time.deltaTime;
        }

        if (elapsedTime > duration)
        {
            elapsedTime = duration;
            gameState.SetGameState(GameplayState.GameOver);
            CheckForAdditionalTime();
        }
        hud.UpdateScoreValue(duration - elapsedTime);


        if (isPass == false)
        {
            loadTime += Time.deltaTime;
            loadMultTime += Time.deltaTime;

            if (loadTime > appearTime)
            {
                if (chankCounter < 35)
                {
                    LoadNextChunk();
                    chankCounter++;
                    ChangeCurrentChunk();
                    loadTime = 0;
                }
                else if (isPlatform)
                {
                    LoadPlatform();
                    isPlatform = false;
                }
            }
           
            if (loadMultTime > appearTime*4)
            {
                LoadnextMult();
                loadMultTime = 0;
            }
        }
       
    }

    public override void ChangedHexState(KindOfMapBehavor mapBehavor)
    {
       
        if (isPass)
        {
            chunk.ChangeHexes(mapBehavor);
        }
        else 
        {
            //foreach (var item in pass)
            //{
            //    item.ChangeHexes(mapBehavor);
            //}
        }
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

    private void LoadPass1()
    {
        isPass = false;

        var nextChunkPos = chunk.Map.Bounds.size.z - hexRadius ;
        //var itemCounter = 0;

        passX = Hexagonal.Cube.HexToPixel(
              Hexagonal.Offset.QToCube(chunk.Map.targetIndex),
              Vector2.one * hexRadius);

        for (int i = 0; i < 20; i++)
        {
            ch = Instantiate(chunkPrefab, this.transform);
            ch.Map.Initializie(new Vector2Int(3, 2), new RectShapePass(), gameParameters.theme);
            ch.Map.gameObject.SetActive(true);

            ch.transform.localPosition = new Vector3(passX.x, 0, nextChunkPos);
      
            pass.Add(ch);
            ch.gameObject.SetActive(false);

            nextChunkPos += ch.Map.Bounds.size.z - hexRadius;

        }

      

        currentChunkIndex = 0;
        pass[currentChunkIndex].gameObject.SetActive(true);
        visiblePass = pass[currentChunkIndex].Map.ToList();

        var bound = new Bounds(new Vector3(passX.x+0.75f, 0, chunk.Map.Bounds.size.z  + 5), new Vector3(2.5f, 0, chunk.Map.Bounds.size.z *10));
       
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
      
        chunkPass.transform.localPosition = new Vector3(passX.x, -3f, nextChunkPos);
        chunkPass.Map.SetTheme(gameParameters.theme);
        chunkPass.gameObject.SetActive(true);
       chunkPass.transform.DOLocalMoveY(0, 0.5f);

        //int[] holes = new[] { 0, 1 };
        //var goUp = chunkPass.Map.Shuffle().Take(Random.Range(0, holes.Length));
        //foreach (var item in goUp)
        //{
        //    item.gameObject.transform.DOLocalMoveY(-3f, 0.5f);
        //}
    }

    private void LoadnextMult()
    {
        if (j < multItems.Count)
        {
            Multiplyer collectMult = Instantiate(multItems[j], Vector3.zero, Quaternion.identity);
            float[] points = new float[3] { passX.x, passX.x + 0.75f, passX.x + 1.5f };

            collectMult.transform.localPosition = new Vector3(points[Random.Range(0, points.Length)], 0.5f, nextChunkPos);
            collectMult.n = j + 2;
            if (j == multItems.Count - 1)
            {
                collectMult.state = PlayerState.BigWin;
                collectMult.transform.localPosition = new Vector3(passX.x + 0.75f, 0.5f, nextChunkPos);
            }
            else
            {
                collectMult.state = PlayerState.Win;
            }
            collectMult.gameObject.SetActive(true);
            visibleMult.Add(collectMult);
        }

        j++;
    }
    private void LoadPlatform()
    {
        platform = Instantiate(chunkPrefab, this.transform);
        platform.Map.Initializie(new Vector2Int(0, 4), new HexShape(), gameParameters.theme);
        platform.Map.gameObject.SetActive(true);

        platform.transform.localPosition = new Vector3(passX.x + 0.75f, 0, nextChunkPos + ch.Map.Bounds.size.z*2.4f - hexRadius);

       
        var throne = Instantiate(thronePrefab, Vector3.zero, Quaternion.identity);
        throne.transform.position = new Vector3(passX.x + 0.75f, -0.35f, nextChunkPos + ch.Map.Bounds.size.z * 2.4f - hexRadius);
        player.thronePlace = throne.transform.position;
        throne.gameObject.SetActive(true);
    }

    private int CheckNextIndex()
    {
        return currentChunkIndex +1  >= pass.Count ? 0 : currentChunkIndex + 1;
    }

    private void ChangeCurrentChunk()
    {
        currentChunkIndex = CheckNextIndex();
    }

    private void OnDisable()
    {
        player.OnPassActivated -= LoadPass1;
    }
}
