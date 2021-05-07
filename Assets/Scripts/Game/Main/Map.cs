using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using static Hexagonal;

public class Map : MonoBehaviour, IEnumerable<Hex>
{
    public const float hexRadius = 0.9755461f / 2;

    [SerializeField] Hex hexPrefab;
    [SerializeField] Renderer planeRenderer;
    [SerializeField] BoxCollider deathTrigger;

    [SerializeField] GameObject arrowPrefab;
    [SerializeField] GameObject safeAreaPrefab;

    private MaterialRepository.Data data;

    private Dictionary<Vector3Int, Hex> hexes = new Dictionary<Vector3Int, Hex>();

    public Hex this[Vector2Int key] => this[Hexagonal.Offset.QToCube(key)];
    public Hex this[Vector3Int key]
    {
        get
        {
            if (hexes.TryGetValue(key, out var hex))
                return hex;
            return null;
        }
    }

    public Vector2Int size
    {
        get;
        private set;
    }

    private Bounds _bounds;
    public Bounds Bounds => _bounds;

    public Vector2Int targetIndex;

    public void Initializie(Vector2Int size, IShape shape, MaterialRepository.Data data)
    {
        this.size = size;
        this.data = data;

        _bounds = new Bounds(transform.position, Vector3.zero);

        hexPrefab.Renderer.material = data.main;
        planeRenderer.material = data.plane;

        foreach (var index in shape.GetIndexes(size))
        {
            Hex hex_go = Instantiate(hexPrefab, Vector3.zero * 2, Quaternion.identity);
            hex_go.index = index;
            hex_go.transform.SetParent(transform);
            hex_go.transform.localPosition = Hexagonal.Cube.HexToPixel(
                index,
                Vector2.one * hexRadius);
            hex_go.name = "Hex_" + index.x + "_" + index.y;
            hexes.Add(index, hex_go);

            _bounds.Encapsulate(hex_go.Renderer.bounds);
        }

        var cent = deathTrigger.transform.InverseTransformPoint(_bounds.center);
        deathTrigger.size = _bounds.size;
        deathTrigger.center = cent + Vector3.down * 0.35f;
    }

    public void SetTheme(MaterialRepository.Data data)
    {
        planeRenderer.material = data.plane;
        foreach (var item in hexes.Values)
        {
            item.Renderer.material = data.main;
        }
    }

    public Hex GetHexByPosition(Vector3 worldPos)
    {
        var hex = Hexagonal.Cube.PixelToHex(
           transform.InverseTransformPoint(worldPos),
           Vector2.one * Map.hexRadius);

        return hexes[hex];
    }

    public void SetThemeWithDelay(MaterialRepository.Data data)
    {
        planeRenderer.material = data.plane;
        StartCoroutine(ChangeTheme(data.main));
    }

    private IEnumerator ChangeTheme(Material mat)
    {
        var list = hexes.Values.ToList();
        int index = 0;

        for (int i = 0; i < list.Count / 10; i++)
        {
            for (int j = 0; j < 10; j++)
            {
                list[index].Renderer.material = mat;
                index++;
            }
            yield return new WaitForSeconds(0.05f);
        }
    }

    public void SetTarget()
    {
        targetIndex = new Vector2Int(
          Random.Range(1, size.x-1),
          Random.Range(size.y - 1, size.y)
          );

        Renderer rend = this[targetIndex]?.Renderer;
        if (rend)
        {
            rend.material = data.target;
            this[targetIndex].IsTarget = true;

            var arrow = Instantiate(arrowPrefab, this[targetIndex].transform);
            arrow.transform.localPosition = Vector3.up;
        }
    }

    public void SetArenaTarget()
    {
       targetIndex = new Vector2Int(0, 0);

        Renderer rend = this[targetIndex]?.Renderer;
        if (rend)
        {
            rend.material = data.target;
            //this[targetIndex].IsTarget = true;
            this[targetIndex].safeZone = true;

            var arrow = Instantiate(arrowPrefab, this[targetIndex].transform);
            arrow.transform.localPosition = Vector3.up;
        }

        var neighbours = Offset.GetQNeighbour(targetIndex);
        foreach (var item in neighbours)
        {
            RedZOnes(item);
        }

        foreach (var item in zones(size))
        {
            var qqq = Offset.QFromCube(item);
            var safeZone = Instantiate(safeAreaPrefab, this[qqq].transform);
            safeZone.transform.localPosition = Vector3.zero;

            RedZOnes(qqq);
            var redZones = Offset.GetQNeighbour(qqq);
            foreach (var ttt in redZones)
            {
                RedZOnes(ttt);
            }
        }
    }

    public void SetSafeZoneArena(Vector2Int vector)
    {
        Renderer rend = this[vector]?.Renderer;
        if (rend)
        {
            rend.material = data.target;
            this[vector].safeZone = true;
        }

        var neighbours = Offset.GetQNeighbour(vector);
        foreach (var ttt in neighbours)
        {
            this[ttt].safeZone = true;
            RedZOnes(ttt);
        }
    }

    public void RedZOnes(Vector2Int vector)
    {
        Renderer _rend = this[vector]?.Renderer;
        if (_rend)
        {
            _rend.material = data.target;
        }
    }

    #region IEnumerable

    public IEnumerator<Hex> GetEnumerator()
        => ((IEnumerable<Hex>)hexes.Values).GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator()
        => ((IEnumerable)hexes.Values).GetEnumerator();

    #endregion


    IEnumerable<Vector3Int> zones(Vector2Int mapSize)
    {
        var m = (int)mapSize.y / 2;

        var indexes = Cube.directions.Select(d => d * m).Append(Vector3Int.zero);
        return indexes;
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(Bounds.center, Bounds.size);
    }

#endif
}
