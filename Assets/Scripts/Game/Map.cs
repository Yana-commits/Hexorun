using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Map : MonoBehaviour, IEnumerable<Hex>
{
    public const float hexRadius = 0.9755461f/2;

    [SerializeField] Hex hexPrefab;
    [SerializeField] Renderer planeRenderer;
    [SerializeField] BoxCollider deathTrigger;
   
    private List<Hex> hexes = new List<Hex>();
    public Vector2Int size
    {
        get;
        private set;
    }
        


    private Bounds _bounds;
    public Bounds Bounds => _bounds;

    public Hex this[Vector2Int index] => hexes[ConvertToArrayIndex(index)];
    public Hex this[int q, int r] => this[new Vector2Int(q,r)];
    private int ConvertToArrayIndex(Vector2Int index) => index.x + index.y * size.x;

    public void Initializie(Vector2Int size, MaterialRepository.Data data)
    {
        this.size = size;
        _bounds = new Bounds(transform.position, Vector3.zero);

        hexPrefab.Renderer.material = data.main;
        planeRenderer.material = data.plane;

        for (int r = 0; r < size.y; r++)
            for (int q = 0; q < size.x; q++)
            {
                Hex hex_go = Instantiate(hexPrefab, Vector3.zero * 2, Quaternion.identity);
                hex_go.index = new Vector2Int(q,r);
                hex_go.transform.SetParent(transform);
                hex_go.transform.localPosition = Hexagonal.Cube.HexToPixel(
                    Hexagonal.Offset.QToCube(hex_go.index),
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
        rend.material = data.target;
        this[targetIndex].IsTarget = true;

        var cent = deathTrigger.transform.InverseTransformPoint(_bounds.center);
        deathTrigger.size = _bounds.size;
        deathTrigger.center = cent + Vector3.down*0.5f;
    }

    public Hex GetHexByPosition(Vector3 worldPos)
    {
        var hex = Hexagonal.Cube.PixelToHex(
           transform.InverseTransformPoint(worldPos),
           Vector2.one * Map.hexRadius);

        return this[Hexagonal.Offset.QFromCube(hex)];
    }

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
