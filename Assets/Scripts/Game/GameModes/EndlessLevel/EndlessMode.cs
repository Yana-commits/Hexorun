using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EndlessMode : Mode
{
    private Player player;
    float hexRadius = 0.9755461f / 2;
    private List<Chunk> chunks = new List<Chunk>();
    private List<Hex> visibleHexes = new List<Hex>();

    float nextChunkPos = 0;
    private float depthFull;
    private float depthHalf;
    private int currentChunkIndex;
    private float generatorTime;
    
    [SerializeField] private float waveDelay = 10;
    private int hexWaveIndex = 0;

    private int themeIndex;

    GameParameters gameParameters = null;

    // Start is called before the first frame update
    void Start()
    {

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

        if (player.transform.position.z >= depthHalf)
        {
            LoadNextChunk();
        }
        if (player.transform.position.z >= depthFull)
            ChangeCurrentChunk();

        waveDelay -= Time.deltaTime;
        if (waveDelay <= 0)
        {
            TryMoveHexDown();
            waveDelay = 0.037f; 
        }

    }

    public override void Initialized(Player _player, HUD hud)
    {
        hud.SetEndlessPanel();
        player = _player;
        float zPos = 0;
        themeIndex = GamePlayerPrefs.LastTheme;

        for (int i = 0; i < levels.Count; i++)
        {
            gameParameters = levels.Parameters[i];
            gameParameters.theme = datas[themeIndex];

            var chunk = Instantiate(chunkPrefab, this.transform);
            chunk.transform.localPosition = new Vector3(0, 0, zPos);
            chunk.Initialize(player.transform, new RectShape(), gameParameters);
            chunk.GeneratePointItem();
            chunks.Add(chunk);
            chunk.gameObject.SetActive(false);
            zPos += chunk.Map.Bounds.size.z - hexRadius;
        }

        currentChunkIndex = 0;       
        chunks[currentChunkIndex].gameObject.SetActive(true);
        depthFull = chunks[currentChunkIndex].Map.Bounds.size.z;
        depthHalf = depthFull / 2;
        visibleHexes = chunks[currentChunkIndex].Map.ToList();

        var hex = chunks[0].Map[new Vector2Int(gameParameters.size.x / 2, 0)];
        var bound = chunks[0].Map.Bounds;
        bound.Expand(new Vector3(0, 0, zPos));
        Vector3 startPos = hex.transform.position;
        player.transform.SetPositionAndRotation(startPos, Quaternion.identity);
        player.SetGamePlaySettings(gameParameters.playerSpeed, bound);
        player.gameObject.SetActive(true);

    }

    private void LoadNextChunk()
    {     
        MaterialRepository.Data theme = datas.Materials[themeIndex];
        nextChunkPos += chunks[currentChunkIndex].Map.Bounds.size.z - hexRadius;
        var chunk = chunks[CheckNextIndex()];
        chunk.transform.localPosition = new Vector3(0, 0, nextChunkPos);
        chunk.Map.SetTheme(theme);
        chunk.gameObject.SetActive(true);
        chunk.ChangeHexes(KindOfMapBehavor.DiffMoove);
        depthHalf = depthHalf + chunk.Map.Bounds.size.z;
        visibleHexes.AddRange(chunk.Map.ToList());
    }

    private void ChangeCurrentChunk()
    {
        currentChunkIndex = CheckNextIndex();
        depthFull += chunks[currentChunkIndex].Map.Bounds.size.z;

        themeIndex = GamePlayerPrefs.LastTheme = (GamePlayerPrefs.LastTheme + 1) % datas.Count;
        MaterialRepository.Data theme = datas.Materials[themeIndex];

        foreach (var item in chunks.GetRange(0,currentChunkIndex + 1))
        {
            item.ChangeChunkTheme(theme);
        }
    }

    public override void ChangedHexState(KindOfMapBehavor mapBehavor)
    {
        foreach (var item in chunks)
        {
            item.ChangeHexes(mapBehavor);
        }
    }

    private int CheckNextIndex()
    {
        return currentChunkIndex + 1 >= chunks.Count ? 0 : currentChunkIndex + 1;
    }

    private void TryMoveHexDown()
    {
        if (visibleHexes.Count < hexWaveIndex)
            return;
        visibleHexes[hexWaveIndex].DisableHex();
        hexWaveIndex++;
    }

}
