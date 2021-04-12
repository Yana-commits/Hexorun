using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Map : MonoBehaviour, IEnumerable<Hex>
{
    public const float hexRadius = 0.9755461f / 2;

    [SerializeField] Hex hexPrefab;
    [SerializeField] Renderer planeRenderer;
    [SerializeField] BoxCollider deathTrigger;

    [SerializeField] GameObject arrowPrefab;

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
            hexes.Add(index,hex_go);

            _bounds.Encapsulate(hex_go.Renderer.bounds);
        }
      
        var cent = deathTrigger.transform.InverseTransformPoint(_bounds.center);
        deathTrigger.size = _bounds.size;
        deathTrigger.center = cent + Vector3.down*0.5f;
    }

    public Hex GetHexByPosition(Vector3 worldPos)
    {
        var hex = Hexagonal.Cube.PixelToHex(
           transform.InverseTransformPoint(worldPos),
           Vector2.one * Map.hexRadius);

        return hexes[hex];
    }

    public void SetTheme(MaterialRepository.Data data)
    {
        planeRenderer.material = data.plane;
        foreach (var item in hexes.Values)
        {
            item.Renderer.material = data.main;
            
        }
       
    }
    public void SetTarget()
    {
        var targetIndex = new Vector2Int(
          Random.Range(0, size.x),
          Random.Range(size.y - 5, size.y - 2)
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
   
    #region IEnumerable

    public IEnumerator<Hex> GetEnumerator()
        => ((IEnumerable<Hex>)hexes.Values).GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator()
        => ((IEnumerable)hexes.Values).GetEnumerator();

    #endregion

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(Bounds.center, Bounds.size);
    }
#endif
}
