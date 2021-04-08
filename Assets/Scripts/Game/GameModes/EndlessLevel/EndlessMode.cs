using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndlessMode : MonoBehaviour
{
    [SerializeField] MaterialRepository datas;
    [SerializeField] LevelRepository levels;
    [SerializeField] GameState gameState;

    [SerializeField] private Chunk chunkPrefab;

    private Player player;
    float hexRadius = 0.9755461f / 2;
    // Start is called before the first frame update
    void Start()
    {

    }

    public void Initialized(Player _player)
    {
        player = _player;
        float zPos = 0;
        GameParameters param;

        for (int i = 0; i < levels.Count; i++)
        {
            param = levels.Parameters[i];
            param.theme = datas[GamePlayerPrefs.LastTheme];
            var chunk = Instantiate(chunkPrefab, this.transform);
            chunk.transform.localPosition = new Vector3(0, 0, zPos);
            chunk.Initialize(player.transform, param);
            zPos += chunk.Map.Bounds.size.z - hexRadius;
        }
    }



    // Update is called once per frame
    void Update()
    {

    }
}
