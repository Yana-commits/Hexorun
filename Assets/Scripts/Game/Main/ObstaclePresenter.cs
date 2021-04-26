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
        foreach (var item in _map)
        {
            HexState state = HexState.None;
            indexes.TryGetValue(Hexagonal.Offset.QFromCube(item.index), out state);
            item.TrySetState(state);
        }
    }

    public void Initialize()
    {
    }
}
