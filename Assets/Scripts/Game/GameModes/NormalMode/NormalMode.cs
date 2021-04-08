using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NormalMode : MonoBehaviour
{
    [SerializeField] MaterialRepository datas;
    [SerializeField] LevelRepository levels;
    [SerializeField] private Chunk chunkPrefab;

    private Player player;
    private GameParameters gameParameters;

    // Start is called before the first frame update
    void Start()
    {

    }

    public void Initialized(Player _player)
    {
        player = _player;
        int level = Mathf.Min(GamePlayerPrefs.LastLevel + 1, levels.Count - 1);
        gameParameters = levels[level];
        gameParameters.id = level;
        gameParameters.theme = datas.Materials[GamePlayerPrefs.LastTheme];

        var chunk = Instantiate(chunkPrefab);
        chunk.Initialize(player.transform, gameParameters);

        var hex = chunk.Map[gameParameters.size.x / 2, 0];
        Vector3 startPos = hex.transform.position;
        player.transform.SetPositionAndRotation(startPos, Quaternion.identity);
        player.SetGamePlaySettings(gameParameters.playerSpeed, chunk.Map.Bounds);
        player.gameObject.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
