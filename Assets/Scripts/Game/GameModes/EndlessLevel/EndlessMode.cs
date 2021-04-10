using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndlessMode : Mode
{  
    private Player player;
    float hexRadius = 0.9755461f / 2;
    private List<Chunk> chunks = new List<Chunk>();

    float nextChunkPos = 0;
    private float depthFull;
    private float depthHalf;
    private int currentChunkIndex;
    // Start is called before the first frame update
    void Start()
    {

    }

    private void Update()
    {
        if (gameState.GamePlayState != GameplayState.Play)
            return;
        if (player.transform.position.z >= depthHalf)
        {
            LoadNextChunk();
        }
        if (player.transform.position.z >= depthFull)
            ChangeCurrentChunk();

    }

    public override void Initialized(Player _player, HUD hud)
    {
        player = _player;
        float zPos = 0;
        GameParameters param = null;

        for (int i = 0; i < levels.Count; i++)
        {
            param = levels.Parameters[i];
            param.theme = datas[GamePlayerPrefs.LastTheme];

            var chunk = Instantiate(chunkPrefab, this.transform);
            chunk.transform.localPosition = new Vector3(0, 0, zPos);
            chunk.Initialize(player.transform, param);       
            chunks.Add(chunk);
            chunk.gameObject.SetActive(false);
            zPos += chunk.Map.Bounds.size.z - hexRadius;
        }

        currentChunkIndex = 0;       
        chunks[currentChunkIndex].gameObject.SetActive(true);
        depthFull = chunks[currentChunkIndex].Map.Bounds.size.z;
        depthHalf = depthFull / 2;

        var hex = chunks[0].Map[param.size.x / 2, 0];
        var bound = chunks[0].Map.Bounds;
        bound.Expand(new Vector3(0, 0, zPos));
        Vector3 startPos = hex.transform.position;
        player.transform.SetPositionAndRotation(startPos, Quaternion.identity);
        player.SetGamePlaySettings(param.playerSpeed, bound);
        player.gameObject.SetActive(true);
    }

    private void LoadNextChunk()
    {   
        nextChunkPos += chunks[currentChunkIndex].Map.Bounds.size.z - hexRadius;
        var chunk = chunks[CheckNextIndex()];
        chunk.transform.localPosition = new Vector3(0, 0, nextChunkPos);     
        chunk.gameObject.SetActive(true);
        chunk.ChangeHexes();
        depthHalf = depthHalf + chunk.Map.Bounds.size.z;
    }

    private void ChangeCurrentChunk()
    {
        currentChunkIndex = CheckNextIndex();
        depthFull += chunks[currentChunkIndex].Map.Bounds.size.z;

        int themeIndex = GamePlayerPrefs.LastTheme = (GamePlayerPrefs.LastTheme + 1) % datas.Count;     
        MaterialRepository.Data theme = datas.Materials[themeIndex];

        foreach (var item in chunks)
        {
            item.ChangeChunkTheme(theme);
        }
    }


    public override void ChangedHexState()
    {
        foreach (var item in chunks)
        {
            item.ChangeHexes();
        }
    }


    private int CheckNextIndex()
    {
        return currentChunkIndex + 1 >= chunks.Count ? 0 : currentChunkIndex + 1;
    }

}
