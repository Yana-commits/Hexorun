using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class ObstaclePresenter : MonoBehaviour
{
    [SerializeField] Map _map;
    [SerializeField] ObstacleGenerator generator;

    private int _holes;

    public void Initialize(int holes) =>
        _holes = holes;

    private void OnEnable()
    {
        generator.ObstaclesGenerated += OnObstaclesGenerated;
    }

    private void OnDisable()
    {
        generator.ObstaclesGenerated -= OnObstaclesGenerated;
    }

    private void OnObstaclesGenerated(IEnumerable<Vector2Int> indexes)
    {
        //TODO:
        var f = Random.Range(0.2f, 0.5f);
        int holes = (int)(f * indexes.Count());

        var hexes = indexes
            .Select(ind => _map[ind])
            .Shuffle()
            .ToArray();

        foreach (var item in hexes.Take(holes))
            item.TrySetState(HexState.DOWN);

        foreach (var item in hexes.Skip(holes))
            item.TrySetState(HexState.UP);

        foreach (var item in _map.Except(hexes))
            item.TrySetState(HexState.NONE);
    }
}
