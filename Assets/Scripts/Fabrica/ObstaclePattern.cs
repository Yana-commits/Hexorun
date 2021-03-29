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
        public abstract void ChangeValue();
    }

    //Proxy
    public class ObstacleCreator : ObstacleFactory
    {
        private static Dictionary<PatternEnum, ObstacleFactory> dict = new Dictionary<PatternEnum, ObstacleFactory>
        {
            [PatternEnum.Wall1] = new WallFirstCreator(),
            [PatternEnum.Wall2] = new WallSecondCreator(),
            [PatternEnum.Wall3] = new WallThirdCreator(),
            [PatternEnum.Path1] = new Path1Creator()
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

    internal class WallFirst : ObstacleProduct
    {
        public override string Id { get; }
        public override int Height => 3;
        public override IDictionary<Vector2Int, HexState> Values { get; }

        private Vector2Int mapSize;
        private Vector2Int offset;
        private Vector2Int currentPoint;
        private readonly HexState pointState = HexState.Hill;

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
                .Select(q => new Vector2Int(q, 1))
                .Shuffle()
                .ToDictionary(ind => ind, ind => pointState);

            var randomPoint = retVal.Keys.Random();
            retVal[randomPoint] = HexState.None;
            currentPoint = randomPoint + offset;

            for (int i = 0; i < mapSize.x; i++)
            {
                retVal.Add(new Vector2Int(i, 0), HexState.None);
                retVal.Add(new Vector2Int(i, Height-1), HexState.None);
            }

            return retVal.ToDictionary(
                pair => pair.Key + offset,
                pair => pair.Value
                );
        }

        public override void ChangeValue()
        {
            var newItem = Values
                .Where(x => x.Key.y == (1 + offset.y) && x.Key != currentPoint)
                .Random();

            Values[currentPoint] = pointState;
            Values[newItem.Key] = HexState.None;
            currentPoint = newItem.Key;
        }
    }

    internal class WallSecond : ObstacleProduct
    {
        public override string Id { get; }
        public override int Height => 3;
        public override IDictionary<Vector2Int, HexState> Values { get; }
        private Vector2Int mapSize;
        private Vector2Int offset;
        private Vector2Int currentPoint;
        private readonly HexState pointState = HexState.Hole;

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
                .Select(q => new Vector2Int(q, 1))
                .Shuffle()
                .ToDictionary(ind => ind, ind => HexState.Hill); // TODO: pointState ?

            currentPoint = retVal.Keys.Random();
            retVal[currentPoint] = HexState.None;

            for (int i = 0; i < mapSize.x; i++)
            {
                retVal.Add(new Vector2Int(i, 0), HexState.None);
                retVal.Add(new Vector2Int(i, Height-1), HexState.None);
            }

            return retVal.ToDictionary(
                pair => pair.Key + offset,
                pair => pair.Value
                );
        }

        public override void ChangeValue()
        {
            Values[currentPoint + offset] = Values[currentPoint + offset] == pointState ? HexState.None : pointState;
        }
    }

    internal class WallThird : ObstacleProduct
    {
        public override string Id { get; }
        public override int Height => 3;
        public override IDictionary<Vector2Int, HexState> Values { get; }
        private Vector2Int mapSize;
        private Vector2Int offset;

        private int evenOdd = 1;
        private readonly HexState pointState = HexState.Hill;

        public WallThird(Vector2Int mapSize, Vector2Int offset)
        {
            this.Id = nameof(WallThird);
            this.mapSize = mapSize;
            this.offset = offset;
            this.Values = Generate();
        }

        private IDictionary<Vector2Int, HexState> Generate()
        {
            var retVal = Enumerable.Range(0, mapSize.x)
                 .Select(q => new Vector2Int(q, 1))
                 .Shuffle()
                 .ToDictionary(ind => ind, ind => pointState);

            foreach (var item in retVal.Keys.ToArray())
            {
                if ((item.x & evenOdd) == (offset.y & evenOdd))
                    retVal[item] = HexState.None;
            }

            for (int i = 0; i < mapSize.x; i++)
            {
                retVal.Add(new Vector2Int(i, 0), HexState.None);
                retVal.Add(new Vector2Int(i, Height-1), HexState.None);
            }

            return retVal.ToDictionary(
                pair => pair.Key + offset,
                pair => pair.Value
                );
        }

        public override void ChangeValue()
        {
            evenOdd = evenOdd == 1 ? 0 : 1;
            var newList = Values.Where(x => x.Key.y == (1 + offset.y)).ToList();
            foreach (var item in newList)
            {
                if ((item.Key.x % 2 == evenOdd))
                    Values[item.Key] = HexState.None;
                else
                    Values[item.Key] = pointState;
            }
        }
    }

    internal class Path1 : ObstacleProduct
    {
        public override string Id { get; }
        public override int Height { get; }
        public override IDictionary<Vector2Int, HexState> Values { get; }

        private Vector2Int mapSize;
        private Vector2Int offset;

        private readonly HexState pointState = HexState.Hole;

        public Path1(Vector2Int mapSize, Vector2Int offset, int height = 5)
        {
            this.Id = nameof(Path1);
            this.mapSize = mapSize;
            this.offset = offset;
            this.Height = height + 2;  // 1+height+1
            this.Values = Generate();
        }

        private IDictionary<Vector2Int, HexState> Generate()
        {
            IDictionary<Vector2Int, HexState> obstaclesField = new Dictionary<Vector2Int, HexState>();

            for (int q = 0; q < mapSize.x; q++)
            {
                obstaclesField.Add(new Vector2Int(q, 0), HexState.None);

                for (int r = 1; r < Height - 1; r++)
                    obstaclesField.Add(new Vector2Int(q, r), pointState);
                
                obstaclesField.Add(new Vector2Int(q, Height - 1), HexState.None);
            }

            var index = new Vector2Int(Random.Range(0, mapSize.x), 1);
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
            } while (index.y < Height - 2);

            foreach (var item in indexToExclude)
            {
                obstaclesField[item] = HexState.None;
            }

            return obstaclesField.ToDictionary(
                pair => pair.Key + offset,
                pair => pair.Value
            );
        }

        public override void ChangeValue()
        {
            
        }
    }

}