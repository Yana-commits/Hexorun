using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Map : MonoBehaviour
{
    [SerializeField] Hex hexPrefab;
    [SerializeField] Material redMaterial;
    public List<Hex> hexes = new List<Hex>();
    private float changeTime = 1;
    float[] points = new float[10] { 0, 0, 0, 0, 0, 0, 0, 0, 0.5f, 1 };
    float[] minusePoints = new float[10] { -3, -3, -3, -3, -3, -3, -3, -3, -3, -3 };


    private float holesNomber;

    private Vector2Int size;

    public Hex this[Vector3Int index] => hexes[ConvertToArrayIndex(index)];
    public Hex this[Vector2Int index] => hexes[ConvertToArrayIndex(index)];

    private int ConvertToArrayIndex(Vector3Int index) => ConvertToArrayIndex(ConvertCoordToAxial(index));
    private int ConvertToArrayIndex(Vector2Int index) => index.x * size.x + index.y;


    void Start()
    {
        holesNomber = Controller.Instance.gameParameters.holes;
        Debug.Log(size);
    }

    public void Initializie(LevelParameters level)
    {
        size = new Vector2Int(level.ZWidth, level.XHeight);

        float xOffset = level.XOffset;
        float zOffset = level.ZOffset;

        for (int x = 0; x < size.y; x++)
        {
            for (int y = 0; y < size.x; y++)
            {
                float yPos = y * zOffset;

                if (x % 2 == 1)
                {
                    yPos += zOffset / 2f;
                }

                float xPos = x * xOffset;

                var hex_go = Instantiate(hexPrefab, new Vector3(xPos, 0, yPos), Quaternion.identity);
                hex_go.name = "Hex_" + x + "_" + y;
                hex_go.cube_coord = ToCube(x, y);

                hex_go.neihbours = GetNeighbour(hex_go.cube_coord);
                //Debug.Log($"{hex_go.neihbours.ToArray()[1]}");

                hex_go.transform.SetParent(transform);
                hexes.Add(hex_go);
            }
        }

        var target = hexes
            .Where(h => h.cube_coord.z > size.x - 10)
            .OrderBy(v => Random.value)
            .First();


        var rend = target.GetComponent<Renderer>();
        rend.materials = new[] { null, redMaterial, redMaterial };
        target.end = false;
    }

    private void FixedUpdate()
    {
        MooveHexes();
    }

    public void MooveHexes()
    {
        if (Controller.Instance.gameState == GameState.Play)
        {
            changeTime = changeTime - 1 * Time.deltaTime;
            if (changeTime <= 0)
            {
                var ignorHexArray = hexes
                    .Where(h => h.permission == false || !h.end).
                    SelectMany(h => GetNeighbour(h.cube_coord))
                    .Select(ind => this[ind]);

                ignorHexArray = hexes.Where(h => h.permission == false || !h.end).Union(ignorHexArray);

                foreach (var item in ignorHexArray)
                {
                    item.Move(new float[] { 0, 0 });
                }

                var tt = hexes.Except(ignorHexArray);
                foreach (var item in tt)
                {
                    item.Move(points);
                }

                var list = hexes.Where(x => x.state == HexState.NONE).Except(ignorHexArray).ToList();

                for (int i = 0; i < holesNomber; i++)
                {
                    int index = Random.Range(0, list.Count);

                    if (list[index].state != HexState.NONE)
                    {
                        i--;
                    }
                    else
                    {
                        list[index].Move(minusePoints);
                    }
                }
                changeTime = Controller.Instance.gameParameters.changesTime;
            }
        }
    }

    static Vector3Int ToCube(int xPos, int yPos)
    {
        var x = xPos;
        var z = yPos;
        var y = -x - z;

        return new Vector3Int(x, y, z);
    }

    private Vector2Int ConvertCoordToAxial(Vector3Int index)
    {
        var q = index.x;
        var r = index.z;
        return new Vector2Int(q, r);
    }


    private IEnumerable<Vector3Int> _directions
    {
        get
        {
            yield return new Vector3Int(1, -1, 0);
            yield return new Vector3Int(1, 0, -1);
            yield return new Vector3Int(0, 1, -1);

            yield return new Vector3Int(-1, 1, 0);
            yield return new Vector3Int(-1, 0, 1);
            yield return new Vector3Int(0, -1, 1);
        }
    }

    public IEnumerable<Vector3Int> GetNeighbour(Hex hex)
        => GetNeighbour(hex.cube_coord);

    public IEnumerable<Vector3Int> GetNeighbour(Vector3Int index)
        => _directions.Select(v => index + v);

}
