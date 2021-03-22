using System;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public class GameParameters
{
    public Vector2Int size = new Vector2Int(10, 20);

    public int duration;
    public float playerSpeed;
    public float changesTime;
    public int holes;
    public MaterialRepository.Data theme;
    public RangedFloat probability = RangedFloat.Value(0, 1);
}

[Serializable]
public class StartGameEvent : UnityEvent<GameParameters>
{ }