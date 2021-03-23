using System;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public class GameParameters
{
    public Vector2Int size = new Vector2Int(10, 20);

    public int duration = 30;
    public float playerSpeed = 2;
    public float changesTime = 2;
    public RangedFloat obstacleProbability = RangedFloat.Value(0, 1);
    public RangedFloat holeProbability = RangedFloat.Value(0, 1);
    [HideInInspector]
    public MaterialRepository.Data theme;
}

[Serializable]
public class StartGameEvent : UnityEvent<GameParameters>
{ }