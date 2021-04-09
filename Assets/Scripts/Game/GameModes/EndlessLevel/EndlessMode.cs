using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndlessMode : Mode
{
    [SerializeField] GameState gameState;

    private Player player;
    float hexRadius = 0.9755461f / 2;
    private List<Chunk> chunks = new List<Chunk>();
    // Start is called before the first frame update
    void Start()
    {

    }

    public override void Initialized(Player _player)
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
            zPos += chunk.Map.Bounds.size.z - hexRadius;
        }

        var hex = chunks[0].Map[param.size.x / 2, 0];
        var bound = chunks[0].Map.Bounds;
        bound.Expand(new Vector3(0, 0, zPos));
        Vector3 startPos = hex.transform.position;
        player.transform.SetPositionAndRotation(startPos, Quaternion.identity);
        player.SetGamePlaySettings(param.playerSpeed, bound);
        player.gameObject.SetActive(true);
    }

    public override void ChangedHexState()
    {
        foreach (var item in chunks)
        {
            item.ChangeHexes();
        }
    }

}
