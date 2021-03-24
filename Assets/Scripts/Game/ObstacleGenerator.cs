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

    public event Action<IEnumerable<Vector2Int>> ObstaclesGenerated;

    private Transform _player;
    private RangedFloat _obstacleProb;

    public void Initialize(Transform player, RangedFloat obstacleProbability)
    {
        _player = player;
        _obstacleProb = obstacleProbability;
    }

    internal void Generate()
    {
        SimpleGenerator();

        
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        //Gizmos.color = Color.red;
        //Gizmos.DrawSphere(_player.position, _overlapSphereRadius);
    }
#endif

    public void SimpleGenerator()
    {
        //var obstacles = RandomObstacles();
        var obstacles = Pattern3();
        //var obstacles = Pattern2();
        //var obstacles = Pattern1();

        ObstaclesGenerated?.Invoke(obstacles);
    }

    private IEnumerable<Vector2Int> RandomObstacles()
    {
        var obstacles = _map
               .Shuffle()
               .Take((int)(_map.Count() * _obstacleProb.Random()))
               //.Where(hex => Random.Range(0, 5) == 0) // 1/5 = 20%
               .Select(h => h.index);

        //TODO: если клетка опущена Physics.OverlapSphere не может ее отловить
        var colliders = Physics.OverlapSphere(_player.position, _overlapSphereRadius, hexLayer);

        var hexIndexes = colliders.Select(c => c.GetComponent<Hex>().index);

       return obstacles.Except(hexIndexes);
    }

    private IEnumerable<Vector2Int> Pattern1()
    {
        var hex = _map.GetHexByPosition(_player.position).index;

        return Enumerable.Range(0, _map.size.x)
            .Select(q => new Vector2Int(q, hex.y + 2))
            .Shuffle()
            .Skip(1);
        /*
        int skip = (int)(Random.value * _map.size.x);
        for (int q = 0; q < _map.size.x; q++)
        {
            if (q == skip) continue;
            yield return new Vector2Int(q, 0 + offsetR);
        }
        */
    }

    private IEnumerable<Vector2Int> Pattern2()
    {
        var hex = _map.GetHexByPosition(_player.position).index;

        //return Enumerable.Range(0, _map.size.x)
        //    .Select(q => new Vector2Int(q, hex.y + 1))
        //    .Where(ind => (ind.x & 1) == (hex.y & 1));

        if ((hex.y + 2) % 2 == 0)
        {
            for (int q = 0; q < _map.size.x; q = q + 2)
            {
                yield return new Vector2Int(q, hex.y + 2);
            }
        }
        else
        {
            for (int q = 1; q < _map.size.x; q = q + 2)
            {
                yield return new Vector2Int(q, hex.y + 2);
            }
        }

    }

    private IEnumerable<Vector2Int> Pattern3()
    {
        var hex = _map.GetHexByPosition(_player.position).index;

        List<Vector2Int> obstaclesField = new List<Vector2Int>();

        for (int q = 0; q < _map.size.x; q++)
        {
            for (int r = 0; r < 5; r++)
            {
                obstaclesField.Add(new Vector2Int(q, r));
            }
        }

        var index = new Vector2Int(Random.Range(0, _map.size.x), 0);
        List<Vector2Int> indexToExclude = new List<Vector2Int>();
        indexToExclude.Add(index);

        while (index.y < 5)
        {
            Vector2Int neighbor = Vector2Int.one;
            indexToExclude.Add(neighbor);
            index = neighbor;
        }

        return obstaclesField
            .Except(indexToExclude)
            .Select(ind => ind + new Vector2Int(0, hex.y+1));
    }
}
