using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ObstaclePresenter : MonoBehaviour
{
    [SerializeField] Map _map;
    [SerializeField] ObstacleGenerator generator;

    private void OnEnable()
    {
        generator.ObstaclesGenerated += OnObstaclesGenerated;
    }

    private void OnDisable()
    {
        generator.ObstaclesGenerated -= OnObstaclesGenerated;
    }

    private void OnObstaclesGenerated(IDictionary<Vector2Int, HexState> indexes)
    {
        var hexes = indexes
            .Select(ind => _map[ind.Key])
            .Shuffle()
            .ToArray();

        foreach (var item in hexes)
            item.TrySetState(indexes[item.index]);
        
        foreach (var item in _map.Except(hexes))
            item.TrySetState(HexState.None);
    }

    public void Initialize()
    {
    }
}
