using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public interface IShape
{
    IEnumerable<Vector3Int> GetIndexes(Vector2Int size);
}

public class RectShape : IShape
{
    public IEnumerable<Vector3Int> GetIndexes(Vector2Int size)
    {
        for (int r = 0; r < size.y; r++)
            for (int q = 0; q < size.x; q++)
                yield return Hexagonal.Offset.QToCube(new Vector2Int(q,r), OffsetCoord.Even);
    }
}

public class HexShape : IShape
{
    public IEnumerable<Vector3Int> GetIndexes(Vector2Int size)
    {
        var radius = size.y;
        for (int q = -radius; q <= radius; q++)
        {
            int r1 = Mathf.Max(-radius, -q - radius);
            int r2 = Mathf.Min(radius, -q + radius);
            for (int r = r1; r <= r2; r++)
                yield return new Vector3Int(q, r, -q - r);
        }
    }
}
