using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public class GameParameters
{
    [Serializable]
    public class Obstacles
    {
        public RangedFloat obstacleProbability;
        public RangedFloat holeProbability;
        public List<PatternEnum> pattern;

        public Obstacles()
        {
            obstacleProbability = RangedFloat.Value(0, 1);
            holeProbability = RangedFloat.Value(0, 1);
            pattern = new List<PatternEnum>();
        }
    }

    public Vector2Int size;

    public int duration;
    public float playerSpeed;
    public float changesTime;

    [NonSerialized]
    public MaterialRepository.Data theme;
    [NonSerialized]
    public int id;

    public List<KeyValuePair<GameObject,int>> collectableItems;
    public Obstacles obstaclesParam;

    public GameParameters()
    {
        size = new Vector2Int(10, 20);
        duration = 30;
        playerSpeed = 2;
        changesTime = 2;
        theme = default;
        collectableItems = new List<KeyValuePair<GameObject, int>>();
    }
}

[Serializable]
public struct KeyValuePair<TKey, TValue>
{
    public TKey Key;
    public TValue Value;
}

[Serializable]
public class StartGameEvent : UnityEvent<GameParameters>
{ }