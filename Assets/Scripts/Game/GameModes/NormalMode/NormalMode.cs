using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NormalMode : Mode
{
    private Player player;
    private GameParameters gameParameters;
    private Chunk chunk;

    // Start is called before the first frame update
    void Start()
    {
       
    }

    public override void Initialized(Player _player)
    {
        player = _player;
        int level = Mathf.Min(GamePlayerPrefs.LastLevel + 1, levels.Count - 1);
        gameParameters = levels[level];
        gameParameters.id = level;
        gameParameters.theme = datas.Materials[GamePlayerPrefs.LastTheme];

        chunk = Instantiate(chunkPrefab);
        chunk.Initialize(player.transform, gameParameters);

        var hex = chunk.Map[gameParameters.size.x / 2, 0];
        Vector3 startPos = hex.transform.position;
        player.transform.SetPositionAndRotation(startPos, Quaternion.identity);
        player.SetGamePlaySettings(gameParameters.playerSpeed, chunk.Map.Bounds);
        player.gameObject.SetActive(true);
    }

    public override void ChangedHexState()
    {
        chunk.ChangeHexes();
    }

}

public abstract class Mode : MonoBehaviour
{
    public MaterialRepository datas;
    public LevelRepository levels;
    public Chunk chunkPrefab;

    public abstract void Initialized(Player _player);
    public abstract void ChangedHexState();
}