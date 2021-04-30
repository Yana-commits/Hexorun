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
    private HexState hexState = HexState.None;


    //public void Initialize(Transform player, GameParameters.Obstacles obstaclesParam)
    //{
    //    patterns = new List<ObstacleProduct>();
    //    _player = player;
    //    _obstaclesParam = obstaclesParam;


    //}

    public void Initialize(Transform player, IShape shape, GameParameters.Obstacles obstaclesParam)
    {
        patterns = new List<ObstacleProduct>();
        _player = player;
        _obstaclesParam = obstaclesParam;


        if (shape is RectShape)
        {
            IEnumerable<ObstacleFactory> creators = _obstaclesParam.pattern

                .Select(p => new ObstacleCreator(p))
                .Shuffle();

            int patternsHeight = creators.Sum(c => c.Generate(_map.size, Vector2Int.zero).Height);
            int range = _map.size.y - patternsHeight - 10;

            int offset = 5;
            foreach (var factory in creators)
            {
                int ttt = Random.Range(0, range);
                range -= ttt;
                offset += ttt;

                var obstacle = factory.Generate(_map.size, Vector2Int.up * offset);
                offset += obstacle.Height;
                patterns.Add(obstacle);
            }
        }
        else
        {
            var obstacle = new ObstacleCreator(PatternEnum.RoundMapZones);
            int offset = 0;
            var patObstacle = obstacle.Generate(_map.size, Vector2Int.up * offset);
            patterns.Add(patObstacle);
        }

    }

    internal void Generate(KindOfMapBehavor mapBehavor)
    {
        SimpleGenerator(mapBehavor);
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        //Gizmos.color = Color.red;
        //Gizmos.DrawSphere(_player.position, _overlapSphereRadius);
    }
#endif
    public void SimpleGenerator(KindOfMapBehavor mapBehavor)
    {
        Dictionary<Vector2Int, HexState> hexObstacles = new Dictionary<Vector2Int, HexState>();

        if (mapBehavor == KindOfMapBehavor.DiffMoove)
        {
            DiffDirections(hexObstacles);
        }
        else
        {
            AllDown(hexObstacles);
        }

        var patt = patterns.SelectMany(p => p.GetValues());

        foreach (var item in patt)
            hexObstacles[item.Key] = item.Value;

        ObstaclesGenerated?.Invoke(hexObstacles);

        foreach (var item in patterns)
            item.ChangeValue();

    }

    public void DiffDirections(Dictionary<Vector2Int, HexState> hexObstacles)
    {
        var randomObstacles = RandomObstacles();

        foreach (var item in randomObstacles)
            hexObstacles.Add(item, HexState.Hill);

        int holes = (int)(_obstaclesParam.holeProbability.Random() * randomObstacles.Count());
        var holesIndexes = randomObstacles.Shuffle().Take(holes);
        foreach (var item in holesIndexes)
            hexObstacles[item] = HexState.Hole;
    }

    private void AllDown(Dictionary<Vector2Int, HexState> hexObstacles)
    {
        var randomObstacles = AllObstacles();

        hexState = hexState == HexState.None ? HexState.Hole : HexState.None;

        foreach (var item in randomObstacles)
            hexObstacles.Add(item, hexState);

        hexState = HexState.None;
    }

    private IEnumerable<Vector2Int> AllObstacles()
    {
        //return new Vector2Int[0];
        var obstacles = _map.Select(h => h.index);

        return obstacles
            .Select(ind => Offset.QFromCube(ind));
    }

    private IEnumerable<Vector2Int> RandomObstacles()
    {
       
        //return new Vector2Int[0];
        var obstacles = _map
               .Shuffle()
               .Take((int)(_map.Count() * _obstaclesParam.obstacleProbability.Random()))
               .Select(h => h.index);
      
        //TODO: если клетка опущена Physics.OverlapSphere не может ее отловить
        var colliders = Physics.OverlapSphere(_player.position, _overlapSphereRadius, hexLayer);
        var hexIndexes = colliders.Select(c => c.GetComponent<Hex>().index);

        return obstacles
             .Except(hexIndexes)
             .Select(ind => Offset.QFromCube(ind));
    }

}
