using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Factory.ObstaclePattern;
using static Hexagonal;
using Random = UnityEngine.Random;

public class ObstacleGenerator : MonoBehaviour
{
    [SerializeField] Map _map;
    [Space]
    [SerializeField] LayerMask hexLayer;
    [SerializeField] float _overlapSphereRadius = 0.5f;

    public event Action<IDictionary<Vector2Int, HexState>> ObstaclesGenerated;

    private Transform _player;
    private GameParameters.Obstacles _obstaclesParam;

    private List<ObstacleProduct> patterns;
 

    public void Initialize(Transform player, GameParameters.Obstacles obstaclesParam)
    {
        patterns = new List<ObstacleProduct>();
        _player = player;
        _obstaclesParam = obstaclesParam;

        var creators = _obstaclesParam.pattern
            .Select(p => new ObstacleCreator(p))
            .Shuffle();

        int patternsHeight = creators.Sum(c => c.Generate(_map.size, Vector2Int.zero).Height);
        int range = _map.size.y - patternsHeight - 10;

        int offset = 5;
        foreach (var factory in creators)
        {
            int ttt = Random.Range(0, range);
            range -= ttt - 1;
            offset += ttt;
            
            var obstacle = factory.Generate(_map.size, Vector2Int.up * offset);
            offset += obstacle.Height+1;
            patterns.Add(obstacle);
        }
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
        Dictionary<Vector2Int, HexState> hexObstacles = new Dictionary<Vector2Int, HexState>();

      
        var randomObstacles = RandomObstacles();

        foreach (var item in randomObstacles)
            hexObstacles.Add(item, HexState.Hill);

        int holes = (int)(_obstaclesParam.holeProbability.Random() * randomObstacles.Count());
        var holesIndexes = randomObstacles.Shuffle().Take(holes);
        foreach (var item in holesIndexes)
            hexObstacles[item] = HexState.Hole;

        foreach (var item in patterns)
        {
            item.ChangeValue();
        }
        var patt = patterns.SelectMany(p => p.Values);

        foreach (var item in patt)
            hexObstacles[item.Key] = item.Value;

        ObstaclesGenerated?.Invoke(hexObstacles);
    }

    private IEnumerable<Vector2Int> RandomObstacles()
    {
        var obstacles = _map
               .Shuffle()
               .Take((int)(_map.Count() * _obstaclesParam.obstacleProbability.Random()))
               //.Where(hex => Random.Range(0, 5) == 0) // 1/5 = 20%
               .Select(h => h.index);

        //TODO: если клетка опущена Physics.OverlapSphere не может ее отловить
        var colliders = Physics.OverlapSphere(_player.position, _overlapSphereRadius, hexLayer);

        var hexIndexes = colliders.Select(c => c.GetComponent<Hex>().index);

       return obstacles.Except(hexIndexes);
    }

    public int OffsetY()
    {
        var hex = _map.GetHexByPosition(_player.position).index;
        int offsetY = Mathf.Clamp(hex.y + 2, 5, _map.size.y - 5);

        return offsetY;
    }
}
