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
                yield return Hexagonal.Offset.QToCube(new Vector2Int(q,r));
    }
}

public class HexShape : IShape
{
    public IEnumerable<Vector3Int> GetIndexes(Vector2Int size)
    {
        for (int q = -size.x; q <= size.x; q++)
        {
            int r1 = Mathf.Max(-size.x, -q - size.x);
            int r2 = Mathf.Min(size.x, -q + size.x);
            for (int r = r1; r <= r2; r++)
                yield return new Vector3Int(q, r, -q - r);
        }
    }
}
