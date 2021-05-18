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
    private GameObject throne;

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

    private bool isPass = true;
    private int chankCounter = 0;
    private int j = 0;

    [SerializeField]
    private float appearTime;

    // Start is called before the first frame update
    void Start()
    {
        //player.OnPassActivated += LoadPass;

    }

    public override void Initialized(Player _player, HUD hud)
    {
        player = _player;
        this.hud = hud;
        hud.SetActiveNormalPanel();

        duration = gameParameters.duration;

        chunk = Instantiate(chunkPrefab, this.transform);
        chunk.Initialize(player.transform, new RectShape(), gameParameters);
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
            if (loadTime > appearTime)
            {
                if (chankCounter < pass.Count)
                {
                    LoadNextChunk();
                    chankCounter++;
                    ChangeCurrentChunk();
                    loadTime = 0;
                }
            }
        }

    }

    public override void ChangedHexState(KindOfMapBehavor mapBehavor)
    {
        if (isPass)
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

    public void LoadPass()
    {
        isPass = false;

         nextChunkPos = chunk.Map.Bounds.size.z - (chunk.Map.targetIndex.x % 2 > 0 ? hexRadius : 0);

        passX = Hexagonal.Cube.HexToPixel(
              Hexagonal.Offset.QToCube(chunk.Map.targetIndex),
              Vector2.one * hexRadius) - Vector3.right * (1.5f * hexRadius);

        for (int i = 0; i < 9; i++)
        {
            ch = Instantiate(chunkPrefab, this.transform);
            ch.Map.Initializie(new Vector2Int(3, 5), new RectShapePass(), gameParameters.theme);
            ch.Map.gameObject.SetActive(true);
            ch.Map.DisablePlaneRender();
            ch.Map.SetDeathColliderPosition(new Vector3(0, 0.15f,0));
            ch.transform.localPosition = new Vector3(passX.x, 0, nextChunkPos);

            pass.Add(ch);
            ch.gameObject.SetActive(false);

            nextChunkPos += ch.Map.Bounds.size.z - hexRadius;
        }

        LoadPlatform();

        currentChunkIndex = 0;
        pass[currentChunkIndex].gameObject.SetActive(true);
        visiblePass = pass[currentChunkIndex].Map.ToList();
        visiblePass[1].gameObject.SetActive(chunk.Map.targetIndex.x % 2 > 0 ? false : true);

        var bound = new Bounds(new Vector3(passX.x + 0.75f, 0, chunk.Map.Bounds.size.z + 5), new Vector3(2.5f, 0, chunk.Map.Bounds.size.z * 10));

        player.SetGamePlaySettings(gameParameters.playerSpeed + 1, bound);
    }

    private void LoadNextChunk()
    {
        var chunkPass = pass[CheckNextIndex()];
        chunkPass.Map.SetTheme(gameParameters.theme);
        chunkPass.gameObject.SetActive(true);
        var list = chunkPass.Map.ToList();

        int line = ((list.Count / 3) / 2);
        int holeIndex = Random.Range(line * 3, (line * 3) + 3);

        float delay = 0f;
        for (int i = 0; i < list.Count; i++)
        {
            list[i].transform.localPosition += new Vector3(0, -3f, 0);
            if (i != holeIndex)
                list[i].gameObject.transform.DOLocalMoveY(0f, 0.2f).SetDelay(delay);
            delay += 0.046f;
        }
        LoadNextMult();
        if (currentChunkIndex == pass.Count - 1)
        {
            platform.gameObject.SetActive(true);
            throne.SetActive(true);
        }
            

    }

    private void LoadNextMult()
    {
        if (j < multItems.Count)
        {
            Multiplyer collectMult = Instantiate(multItems[j], Vector3.zero, Quaternion.identity);

            var list = pass[currentChunkIndex].Map.ToList();
            int line = ((list.Count / 3));

            int multiIndex = (j == multItems.Count - 1) ? (line * 3) - 2 : Random.Range((line * 3) - 3, line * 3);
            collectMult.transform.SetParent(list[multiIndex].transform);
            collectMult.transform.localPosition = new Vector3(0, 0.5f, 0);
            collectMult.n = j + 2;
            collectMult.state = (j == multItems.Count - 1) ? PlayerState.BigWin : PlayerState.Win;
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
        platform.transform.localPosition = new Vector3(passX.x + 0.75f, 0, nextChunkPos + (platform.Map.Bounds.size.z / 2) - hexRadius * 2);
        platform.Map.DisableDeathCollider();
        platform.Map.DisablePlaneRender();
        platform.gameObject.SetActive(false);

        throne = Instantiate(thronePrefab, Vector3.zero, Quaternion.identity);
        throne.transform.position = new Vector3(passX.x + 0.75f, -0.35f, nextChunkPos + (platform.Map.Bounds.size.z / 2) - hexRadius * 2);
        player.thronePlace = throne.transform.position;
        throne.gameObject.SetActive(false);
    }

    private int CheckNextIndex()
    {
        return currentChunkIndex + 1 >= pass.Count ? 0 : currentChunkIndex + 1;
    }

    private void ChangeCurrentChunk()
    {
        currentChunkIndex = CheckNextIndex();
    }

    private void OnDisable()
    {
        //player.OnPassActivated -= LoadPass;
    }
}
