using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
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
    private RangedFloat _obstacleProb;
    private RangedFloat _holes;

    public void Initialize(Transform player, RangedFloat obstacleProbability, RangedFloat holeProbability)
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
        Dictionary<Vector2Int, HexState> hexObstacles = new Dictionary<Vector2Int, HexState>();

        /*
        var randomObstacles = RandomObstacles();

        foreach (var item in randomObstacles)
            hexObstacles.Add(item, HexState.Hill);

        int holes = (int)(_holes.Random() * randomObstacles.Count());
        var holesIndexes = randomObstacles.Shuffle().Take(holes);
        foreach (var item in holesIndexes)
            hexObstacles[item] = HexState.Hole;
        */
        foreach (var item in Pattern2())
        {
            hexObstacles[item.Key] = item.Value;
        }

        ObstaclesGenerated?.Invoke(hexObstacles);
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

    private IDictionary<Vector2Int, HexState> Pattern1()
    {
        var retVal = Enumerable.Range(0, _map.size.x)
            .Select(q => new Vector2Int(q, OffsetY()))
            .Shuffle()
            .ToDictionary(ind => ind, ind => HexState.Hill);

        retVal[retVal.Keys.Random()] = HexState.None;
        return retVal;

        /*
        int skip = (int)(Random.value * _map.size.x);
        for (int q = 0; q < _map.size.x; q++)
        {
            if (q == skip) continue;
            yield return new Vector2Int(q, 0 + offsetR);
        }
        */
    }

    private IDictionary<Vector2Int, HexState> Pattern2()
    {
        var offset = new Vector2Int(0, OffsetY());

        var retVal = Enumerable.Range(0, _map.size.x)
            .Select(q => new Vector2Int(q, 0))
            .Shuffle()
            .ToDictionary(ind => ind, ind => HexState.Hill);

        foreach (var item in retVal.Keys.ToArray())
        {
            if ((item.x & 1) == (offset.y & 1))
                retVal[item] = HexState.None;
        }

/*
        if ((hex.y + 2) % 2 == 0)
        {
            for (int q = 0; q < _map.size.x; q = q ++)
            {
             var wall = new Vector2Int(q, OffsetY());
                if(q % 2 !=0)
                obstacles.Add(wall, HexState.Hill);
                else
                 obstacles.Add(wall, HexState.None);
            }
        }
        else
        {
            for (int q = 0; q < _map.size.x; q = q++)
            {
                var wall = new Vector2Int(q, OffsetY());
                if (q % 2 == 0)
                    obstacles.Add(wall, HexState.Hill);
                else
                    obstacles.Add(wall, HexState.None);
            }
        }
*/
        return retVal.ToDictionary(pair => pair.Key + offset, pair => pair.Value); ;
    }

    private IDictionary<Vector2Int, HexState> Pattern3()
    {
        IDictionary<Vector2Int, HexState> obstaclesField = new Dictionary<Vector2Int, HexState>();
       
        for (int q = 0; q < _map.size.x; q++)
        {
            for (int r = 0; r < 5; r++)
            {
                obstaclesField.Add(new Vector2Int(q, r), HexState.Hill);
            }
        }

        var index = new Vector2Int(Random.Range(0, _map.size.x), 0);
        HashSet<Vector2Int> indexToExclude = new HashSet<Vector2Int>();
        indexToExclude.Add(index);

        IEnumerable<Vector2Int> GetPassable(Vector2Int start, int width)
        {
            return Offset.GetQNeighbour(start)
                .Where(d => (d - start).y >= (start.x & 1))
                .Where(n => n.x >= 0 && n.x < width);
        }

        do
        {
            var neighbor = GetPassable(index, _map.size.x).Random();
            if (indexToExclude.Add(neighbor))
                index = neighbor;
        } while (index.y < 4);

        foreach (var item in indexToExclude)
        {
            obstaclesField[item] = HexState.None;
        }

        var offset = new Vector2Int(0, OffsetY());

        return obstaclesField.ToDictionary(pair => pair.Key + offset, pair => pair.Value);
    }

    public int OffsetY()
    {
        var hex = _map.GetHexByPosition(_player.position).index;
        int offsetY = Mathf.Clamp(hex.y + 2, 5, _map.size.y - 5);

        return offsetY;
    }
}
