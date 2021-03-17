using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

using Random = UnityEngine.Random;

public class ObstacleGenerator : MonoBehaviour
{
    [SerializeField] Map _map;
    [Space]
    [SerializeField] LayerMask hexLayer;
    [SerializeField] float _overlapSphereRadius = 0.5f;

    public Action<IEnumerable<Vector2Int>> ObstaclesGenerated;
    private Transform _player;

    public void Initialize(Transform player)
    {
        _player = player;
    }

    internal void Generate()
    {
        var obstacles = _map
            .Where(hex => Random.Range(0, 5) == 0) // 1/5 = 20%
            .Select(h => h.index);

        //TODO: если клетка опущена Physics.OverlapSphere не может ее отловить
        var colliders = Physics.OverlapSphere(_player.position, _overlapSphereRadius, hexLayer);

        var hexIndexes = colliders.Select(c => c.GetComponent<Hex>().index);

        obstacles = obstacles.Except(hexIndexes);

        ObstaclesGenerated?.Invoke(obstacles);
    }
}
