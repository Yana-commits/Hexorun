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

    public abstract class ObstacleProduct
    {
        public abstract string Id { get; }
        public abstract int Height { get; }
        public abstract IDictionary<Vector2Int, HexState> Values { get; }
    }

    //Proxy
    public class ObstacleCreator : ObstacleFactory
    {
        private static Dictionary<PatternEnum, ObstacleFactory> dict = new Dictionary<PatternEnum, ObstacleFactory>
        {
            [PatternEnum.Wall1] = new WallFirstCreator(),
            [PatternEnum.Wall2] = new WallSecondCreator(),
            [PatternEnum.Wall3] = new WallThirdCreator()
        };

        private ObstacleFactory factory;

        public ObstacleCreator(PatternEnum type)
        {
            factory = dict[type];
            
        }

        public override ObstacleProduct Generate(Vector2Int mapSize, Vector2Int offset)
        {
            return factory.Generate(mapSize, offset);
        }
    }

    internal class WallFirstCreator : ObstacleFactory
    {
        public override ObstacleProduct Generate(Vector2Int mapSize, Vector2Int offset)
        {
            return new WallFirst(mapSize, offset);
        }
    }

    internal class WallSecondCreator : ObstacleFactory
    {
        public override ObstacleProduct Generate(Vector2Int mapSize, Vector2Int offset)
        {
            return new WallSecond(mapSize, offset);
        }
    }

    internal class WallThirdCreator : ObstacleFactory
    {
        public override ObstacleProduct Generate(Vector2Int mapSize, Vector2Int offset)
        {
            return new WallThird(mapSize, offset);
        }
    }

    internal class WallFirst : ObstacleProduct
    {
        public override string Id { get; }
        public override int Height => 2;
        public override IDictionary<Vector2Int, HexState> Values { get; }
        private Vector2Int mapSize;
        private Vector2Int offset;

        public WallFirst(Vector2Int mapSize, Vector2Int offset)
        {
            this.Id = nameof(WallFirst);
            this.mapSize = mapSize;
            this.offset = offset;
            this.Values = Generate();
        }

        private IDictionary<Vector2Int, HexState> Generate()
        {
            var retVal = Enumerable.Range(0, mapSize.x)
                .Select(q => new Vector2Int(q, 0))
                .Shuffle()
                .ToDictionary(ind => ind, ind => HexState.Hill);

            retVal[retVal.Keys.Random()] = HexState.None;

            
            return retVal.ToDictionary(
                pair => pair.Key + offset,
                pair => pair.Value
                );
        }
    }

    internal class WallSecond : ObstacleProduct
    {
        public override string Id { get; }
        public override int Height => 2;
        public override IDictionary<Vector2Int, HexState> Values { get; }
        private Vector2Int mapSize;
        private Vector2Int offset;

        public WallSecond(Vector2Int mapSize, Vector2Int offset)
        {
            this.Id = nameof(WallSecond);
            this.mapSize = mapSize;
            this.offset = offset;
            this.Values = Generate();
        }

        private IDictionary<Vector2Int, HexState> Generate()
        {
            var retVal = Enumerable.Range(0, mapSize.x)
                 .Select(q => new Vector2Int(q, 0))
                 .Shuffle()
                 .ToDictionary(ind => ind, ind => HexState.Hill);

            foreach (var item in retVal.Keys.ToArray())
            {
                if ((item.x & 1) == (offset.y & 1))
                    retVal[item] = HexState.None;
            }

            return retVal.ToDictionary(
                pair => pair.Key + offset,
                pair => pair.Value
                );
        }
    }

    internal class WallThird : ObstacleProduct
    {
        public override string Id { get; }
        public override int Height { get; }
        public override IDictionary<Vector2Int, HexState> Values { get; }

        private Vector2Int mapSize;
        private Vector2Int offset;

        public WallThird(Vector2Int mapSize, Vector2Int offset, int height = 5)
        {
            this.Id = nameof(WallThird);
            this.mapSize = mapSize;
            this.offset = offset;
            this.Height = height;
            this.Values = Generate();
        }

        private IDictionary<Vector2Int, HexState> Generate()
        {
            IDictionary<Vector2Int, HexState> obstaclesField = new Dictionary<Vector2Int, HexState>();

            for (int q = 0; q < mapSize.x; q++)
            {
                for (int r = 0; r < Height; r++)
                {
                    obstaclesField.Add(new Vector2Int(q, r), HexState.Hole);
                }
            }

            var index = new Vector2Int(Random.Range(0, mapSize.x), 0);
            HashSet<Vector2Int> indexToExclude = new HashSet<Vector2Int>();
            indexToExclude.Add(index);

            do
            {
                var neighbor = Offset.GetQNeighbour(index)
                    .Where(d => (d - index).y >= (index.x & 1))
                    .Where(n => n.x >= 0 && n.x < mapSize.x)
                    .Random();

                if (indexToExclude.Add(neighbor))
                    index = neighbor;
            } while (index.y < Height - 1);

            foreach (var item in indexToExclude)
            {
                obstaclesField[item] = HexState.None;
            }

            return obstaclesField.ToDictionary(
                pair => pair.Key + offset,
                pair => pair.Value
            );
        }
    }

}