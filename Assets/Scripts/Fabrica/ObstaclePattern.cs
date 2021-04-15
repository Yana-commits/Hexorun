using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static Hexagonal;

namespace Factory.ObstaclePattern
{
    public abstract class ObstacleFactory
    {
        public abstract ObstacleProduct Generate(Vector2Int mapSize, Vector2Int offset);
    }

    #region Creators
    //Proxy
    public class ObstacleCreator : ObstacleFactory
    {
        private static Dictionary<PatternEnum, ObstacleFactory> dict = new Dictionary<PatternEnum, ObstacleFactory>
        {
            [PatternEnum.Wall1] = new WallFirstCreator(),
            [PatternEnum.Wall2] = new WallSecondCreator(),
            [PatternEnum.Wall3] = new WallThirdCreator(),
            [PatternEnum.Path1] = new Path1Creator(),
            [PatternEnum.RoundMapZones] = new RoundMapZonesCreator()
        };

        private readonly ObstacleFactory factory;

        public ObstacleCreator(PatternEnum type) 
            => factory = dict[type];

        public override ObstacleProduct Generate(Vector2Int mapSize, Vector2Int offset) 
            => factory.Generate(mapSize, offset);
    }

    internal class WallFirstCreator : ObstacleFactory
    {
        public override ObstacleProduct Generate(Vector2Int mapSize, Vector2Int offset) 
            => new WallFirst(mapSize, offset);
    }

    internal class WallSecondCreator : ObstacleFactory
    {
        public override ObstacleProduct Generate(Vector2Int mapSize, Vector2Int offset) 
            => new WallSecond(mapSize, offset);
    }

    internal class WallThirdCreator : ObstacleFactory
    {
        public override ObstacleProduct Generate(Vector2Int mapSize, Vector2Int offset) 
            => new WallThird(mapSize, offset);
    }

    internal class Path1Creator : ObstacleFactory
    {
        public override ObstacleProduct Generate(Vector2Int mapSize, Vector2Int offset) 
            => new Path1(mapSize, offset);
    }

    internal class RoundMapZonesCreator : ObstacleFactory
    {
        public override ObstacleProduct Generate(Vector2Int mapSize, Vector2Int offset)
            => new RoundMapZones(mapSize, offset);
    }
    #endregion

    #region Products
    public class ObstacleProduct
    {
        public string Id => GetType().Name;
        public int Height { get; }

        protected IDictionary<Vector2Int, HexState> Values { get; }
        protected Vector2Int mapSize;
        protected Vector2Int offset;
        protected HexState pointState;

        public ObstacleProduct(Vector2Int mapSize, Vector2Int offset, int height, HexState baseState = HexState.Hill)
        {
            this.mapSize = mapSize;
            this.offset = offset;
            this.pointState = baseState;
            this.Height = height + 2;  // 1+height+1
            this.Values = Generate();
        }

        protected virtual IDictionary<Vector2Int, HexState> Generate()
        {
            IDictionary<Vector2Int, HexState> obstaclesField = new Dictionary<Vector2Int, HexState>();

            for (int q = 0; q < mapSize.x; q++)
            {
                for (int r = 0; r < Height; r++)
                {
                    obstaclesField.Add(new Vector2Int(q, r), HexState.None);
                }
            }

            return obstaclesField;
        }

        public virtual void ChangeValue() { }

        public IEnumerable<(Vector2Int Key, HexState Value)> GetValues()
        {
            foreach (var item in Values)
                yield return (item.Key + offset, item.Value);
        }
    }

    internal class WallFirst : ObstacleProduct
    {
        private Vector2Int currentPoint;

        public WallFirst(Vector2Int mapSize, Vector2Int offset)
            : base(mapSize, offset, 3, HexState.Hill)
        { }

        protected override IDictionary<Vector2Int, HexState> Generate()
        {
            IDictionary<Vector2Int, HexState> obstaclesField = base.Generate();

            currentPoint = new Vector2Int(Random.Range(0, mapSize.x), 1);
            for (int q = 0; q < mapSize.x; q++)
            {
                if (q == currentPoint.x)
                    continue;

                Vector2Int key = new Vector2Int(q, 1);
                obstaclesField[key] = pointState;
            }

            return obstaclesField;
        }

        public override void ChangeValue()
        {
            Values[currentPoint] = pointState;

            int col = Enumerable.Range(0, mapSize.x)
                .Where(q => q != currentPoint.x)
                .Random();
            currentPoint.x = col;
            Values[currentPoint] = HexState.None;
        }
    }

    internal class WallSecond : ObstacleProduct
    {
        private Vector2Int currentPoint;

        public WallSecond(Vector2Int mapSize, Vector2Int offset)
            : base(mapSize, offset, 3, HexState.Hill)
        { }

        protected override IDictionary<Vector2Int, HexState> Generate()
        {
            IDictionary<Vector2Int, HexState> obstaclesField = base.Generate();

            currentPoint = new Vector2Int(Random.Range(0, mapSize.x), 1);
            for (int q = 0; q < mapSize.x; q++)
            {
                if (q == currentPoint.x)
                    continue;

                Vector2Int key = new Vector2Int(q, 1);
                obstaclesField[key] = pointState;
            }

            return obstaclesField;
        }

        public override void ChangeValue()
        {
            HexState currentState = Values[currentPoint];
            Values[currentPoint] = currentState == HexState.Hole ? HexState.None : HexState.Hole;
        }
    }

    internal class WallThird : ObstacleProduct
    {
        private int evenOdd = 1;

        public WallThird(Vector2Int mapSize, Vector2Int offset)
            : base(mapSize, offset, 3, HexState.Hill)
        { }

        protected override IDictionary<Vector2Int, HexState> Generate()
        {
            IDictionary<Vector2Int, HexState> obstaclesField = base.Generate();

            for (int q = 0; q < mapSize.x; q++)
            {
                Vector2Int key = new Vector2Int(q, 1);
                if ((key.x & 1) == evenOdd)
                    obstaclesField[key] = pointState;
            }

            return obstaclesField;
        }

        public override void ChangeValue()
        {
            evenOdd = evenOdd == 1 ? 0 : 1;

            for (int q = 0; q < mapSize.x; q++)
            {
                Vector2Int key = new Vector2Int(q, 1);
                if ((key.x & 1) == evenOdd)
                    Values[key] = pointState;
                else
                    Values[key] = HexState.None;
            }
        }
    }

    internal class Path1 : ObstacleProduct
    {
        public Path1(Vector2Int mapSize, Vector2Int offset)
              : base(mapSize, offset, 5, HexState.Hole)
        { }

        protected override IDictionary<Vector2Int, HexState> Generate()
        {
            IDictionary<Vector2Int, HexState> obstaclesField = base.Generate();

            for (int q = 0; q < mapSize.x; q++)
            {
                for (int r = 1; r < Height - 1; r++)
                    obstaclesField[new Vector2Int(q, r)] = pointState;
            }

            var index = new Vector2Int(Random.Range(0, mapSize.x), 1);
            HashSet<Vector2Int> indexToExclude = new HashSet<Vector2Int> { index };

            do
            {
                var neighbor = Offset.GetQNeighbour(index)
                    .Where(d => (d - index).y >= (index.x & 1))
                    .Where(n => n.x >= 0 && n.x < mapSize.x)
                    .Random();

                if (indexToExclude.Add(neighbor))
                    index = neighbor;
            } while (index.y < Height - 2);

            foreach (var item in indexToExclude)
            {
                obstaclesField[item] = HexState.None;
            }

            return obstaclesField;
        }
    }

    internal class RoundMapZones : ObstacleProduct
    {
        public RoundMapZones(Vector2Int mapSize, Vector2Int offset)
                  : base(mapSize, offset, 0, HexState.None)
        { }

        protected override IDictionary<Vector2Int, HexState> Generate()
        {
             IEnumerable<Vector3Int> zones(Vector2Int mapSize)
            {
                var m = (int) mapSize.y / 4;
                
                yield return new Vector3Int(0, 0, 0);
                yield return new Vector3Int(m, -m, 0);
                yield return new Vector3Int(m, 0, -m);
                yield return new Vector3Int(0, m, -m);

                yield return new Vector3Int(-m, m, 0);
                yield return new Vector3Int(-m, 0, m);
                yield return new Vector3Int(0, -m, m);
            }

            var neighbours = new List<Vector3Int>();

            foreach (var item in zones(mapSize))
            {
                neighbours.Add(item);
                var stopZone = Cube.GetNeighbour(item);
                foreach (var n in stopZone)
                {
                    neighbours.Add(n);
                }
            }

            IDictionary<Vector2Int, HexState> obstaclesField = new Dictionary<Vector2Int, HexState>();
            foreach (var item in neighbours)
            {
                var key = Offset.QFromCube(item);
                obstaclesField.Add(key, HexState.Disable);
            }
            return obstaclesField;
        }
    }
    #endregion
}