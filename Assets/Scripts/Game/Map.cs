using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Map : MonoBehaviour, IEnumerable<Hex>
{
    public const float hexRadius = 0.9755461f/2;

    [SerializeField] Hex hexPrefab;
    [SerializeField] Material redMaterial;

    private List<Hex> hexes = new List<Hex>();
    private Vector2Int size;

    private Bounds _bounds;
    public Bounds Bounds => _bounds;

    public Hex this[Vector2Int index] => hexes[ConvertToArrayIndex(index)];
    public Hex this[int q, int r] => this[new Vector2Int(q,r)];
    private int ConvertToArrayIndex(Vector2Int index) => index.x + index.y * size.x;

    public void Initializie(Vector2Int size)
    {
        this.size = size;
        _bounds = new Bounds(transform.position, Vector3.zero);

        for (int r = 0; r < size.y; r++)
            for (int q = 0; q < size.x; q++)
            {
                Hex hex_go = Instantiate(hexPrefab, Vector3.zero * 2, Quaternion.identity);
                hex_go.index = new Vector2Int(q,r);
                hex_go.transform.SetParent(transform);
                hex_go.transform.localPosition = HexToPixel(
                    OffsetQToCube(hex_go.index),
                    Vector2.one * hexRadius);
                hex_go.name = "Hex_" + q + "_" + r;
                hexes.Add(hex_go);

                _bounds.Encapsulate(hex_go.Renderer.bounds);
            }

        var targetIndex = new Vector2Int(
            Random.Range(0, size.x),
            Random.Range(size.y-10, size.y)
            );

        var rend = this[targetIndex].Renderer;
        rend.materials = new[] { null, redMaterial, redMaterial };
        this[targetIndex].IsTarget = true;
    }

    #region Offset

    static public Vector3Int OffsetRToCube(Vector2Int h, OffsetCoord offset = OffsetCoord.Odd)
    {
        int q = h.x - (int)((h.y + (int)offset * (h.y & 1)) / 2);
        int r = h.y;
        int s = -q - r;

        return new Vector3Int(q, r, s);
    }

    static public Vector3Int OffsetQToCube(Vector2Int h, OffsetCoord offset = OffsetCoord.Odd)
    {
        int q = h.x;
        int r = h.y - (int)((h.x + (int)offset * (h.x & 1)) / 2);
        int s = -q - r;
      
        return new Vector3Int(q, r, s);
    }

    // q=x r=y
    static public Vector2Int RoffsetFromCube(Vector3Int h, OffsetCoord offset = OffsetCoord.Odd)
    {
        int col = h.x + (int)((h.y + (int)offset * (h.y & 1)) / 2);
        int row = h.y;
        
        return new Vector2Int(col, row);
    }

    static public Vector2Int QoffsetFromCube(Vector3Int h, OffsetCoord offset = OffsetCoord.Odd)
    {
        int col = h.x;
        int row = h.y + (int)((h.x + (int)offset * (h.x & 1)) / 2);
      
        return new Vector2Int(col, row);
    }

    #endregion

    #region Cube

    static public List<Vector3Int> directions = new List<Vector3Int> {
        new Vector3Int(1, 0, -1), new Vector3Int(1, -1, 0), new Vector3Int(0, -1, 1),
        new Vector3Int(-1, 0, 1), new Vector3Int(-1, 1, 0), new Vector3Int(0, 1, -1)
    };

    static public List<Vector3Int> diagonals = new List<Vector3Int> {
        new Vector3Int(2, -1, -1), new Vector3Int(1, -2, 1), new Vector3Int(-1, -1, 2),
        new Vector3Int(-2, 1, 1), new Vector3Int(-1, 2, -1), new Vector3Int(1, 1, -2)
    };

    static public IEnumerable<Vector3Int> GetNeighbour(Vector3Int index)
        => directions.Select(v => index + v);

    static public IEnumerable<Vector3Int> GetDiagonals (Vector3Int index)
        => diagonals.Select(v => index + v);

    static public int Length(Vector3Int index)
        => (Mathf.Abs(index.x) + Mathf.Abs(index.y) + Mathf.Abs(index.z)) / 2;

    static public int Distance(Vector3Int start, Vector3Int end)
        => Length(end - start);

    static public Vector3 HexToPixel(Vector3Int index, Vector2 size)
    {
        float sqrt3 = Mathf.Sqrt(3f);

        Matrix4x4 OrientationFlat = new Matrix4x4(
            new Vector4(3f / 2 , sqrt3 / 2),
            new Vector4(0      , sqrt3),
            Vector4.zero,
            Vector4.zero
        );
        /*
        Matrix4x4 OrientationPointy = new Matrix4x4(
            new Vector4(sqrt3, 0),
            new Vector4(sqrt3 / 2, 3f/2),
            Vector4.zero,
            Vector4.zero
        );
        */
        var t = OrientationFlat * (Vector3)index;
        t.Scale(size);

        return new Vector3(t.x, 0,t.y);
    }

    //TODO: добавить перевод позиции в индекс хекса

    #endregion

    #region IEnumerable

    public IEnumerator<Hex> GetEnumerator()
        => ((IEnumerable<Hex>)hexes).GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator()
        => ((IEnumerable)hexes).GetEnumerator();

    #endregion

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(Bounds.center, Bounds.size);
    }
#endif
}
