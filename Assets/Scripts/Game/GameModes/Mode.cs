using System;
using UnityEngine;

public abstract class Mode : MonoBehaviour
{
    //public MaterialRepository datas;
    //public LevelRepository levels;
    public Chunk chunkPrefab;
    public GameState gameState;
    [NonSerialized]
    public GameParameters gameParameters;

    public abstract void Initialized(Player _player, HUD hud);
    public abstract void ChangedHexState(KindOfMapBehavor mapBehavor);
}