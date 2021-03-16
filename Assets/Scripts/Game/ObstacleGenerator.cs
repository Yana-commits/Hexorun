using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

using Random = UnityEngine.Random;

public class ObstacleGenerator : MonoBehaviour
{
    [SerializeField] Transform _player;
    [SerializeField] Map _map;
    [Space]
    [SerializeField] LayerMask hexLayer;

    public Action<IEnumerable<Vector2Int>> ObstaclesGenerated;

    public void Initialize(Transform player)
    {
        _player = player;
    }

    internal void Generate()
    {

        var obstacles = _map
            .Where(hex => Random.Range(0, 5) == 0) // 1/5 = 20%
            .Select(h => h.index);

        var colliders = Physics.OverlapSphere(_player.position, 0.5f, hexLayer);

        var hexIndexes = colliders.Select(c => c.GetComponent<Hex>().index);

        obstacles = obstacles.Except(hexIndexes);

        ObstaclesGenerated?.Invoke(obstacles);
    }
}
