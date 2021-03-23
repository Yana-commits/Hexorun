using System;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public class GameParameters
{
    public Vector2Int size;

    public int duration;
    public float playerSpeed;
    public float changesTime;
    public RangedFloat obstacleProbability;
    public RangedFloat holeProbability;
    [NonSerialized]
    public MaterialRepository.Data theme;
    [NonSerialized]
    public int id;

    public GameParameters()
    {
        size = new Vector2Int(10, 20);
        duration = 30;
        playerSpeed = 2;
        changesTime = 2;
        obstacleProbability = RangedFloat.Value(0, 1);
        holeProbability = RangedFloat.Value(0, 1);
        theme = default;
    }
}

[Serializable]
public class StartGameEvent : UnityEvent<GameParameters>
{ }