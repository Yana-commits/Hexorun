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
}

[Serializable]
public class StartGameEvent : UnityEvent<GameParameters>
{ }