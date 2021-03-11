using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Map : MonoBehaviour
{
    [SerializeField] Material redMaterial;
    public List<Hex> hexes = new List<Hex>();
    private float changeTime=1;
    float[] points = new float[10] { 0, 0, 0, 0, 0, 0, 0, 0, 0.5f, 1 };
    float[] minusePoints = new float[10] {-3, -3 ,-3 ,-3 ,-3 ,-3 ,-3 ,-3 ,-3 ,-3};
  

   
    private float holesNomber;

    void Start()
    {
        hexes = Controller.Instance.hexes;
        holesNomber = HUD.Instance.holes.value;
    }

    private void FixedUpdate()
    {
        MooveHexes();
    }

   

    public static Map Create(LevelParameters level, Hex hexPrefab, List<Hex> hexes)
    {
       int zWdth = level.ZWidth;
        int xHeight = level.XHeight;
        float xOffset = level.XOffset;
        float zOffset = level.ZOffset;
        //int holesNomber = level.HolesNomber;

        Vector3 fieldPosition = Vector3.zero;

        var mapPrefab = Resources.Load<Map>("Prefabs/Map");


        var map = Instantiate(mapPrefab, fieldPosition, Quaternion.identity);

        int pointX = Random.Range(1, (int)(xHeight * xOffset - 1));
        int pointY = Random.Range((int)(zWdth * zOffset - 1), (int)(zWdth * zOffset));
        float haight =0;

        for (int x = 0; x < xHeight; x++)
        {
            for (int y = 0; y < zWdth; y++)
            {
                float yPos = y * zOffset;

                if (x % 2 == 1)
                {
                    yPos += zOffset / 2f;
                }

                float xPos = x * xOffset;
                //bool isActive = true;
                //int destiny = Random.Range(0, 100);

                //if (holesNomber > 0 && destiny % 70 == 0)
                //{
                //    haight = -0.5f;
                //    holesNomber--;
                //    isActive = false;
                //}
                //else
                //{
                //    haight = 0;
                //}
                var hex_go = Instantiate(hexPrefab, new Vector3(xPos, haight, yPos), Quaternion.identity) as Hex;

                hex_go.name = "Hex_" + x + "_" + y;

                //var cmp =  hex_go.GetComponent<Hex>();

                if (x == pointX && y == pointY)
                {
                    var rend = hex_go.GetComponent<Renderer>();
                    rend.materials = new[] { null, map.redMaterial };
                    /*
                    foreach (var material in rend.materials)
                    {
                        material.SetColor("Color", Color.red);
                        material.SetColor("Color 2", Color.red);
                    }    
                    */
                    hex_go.end = false;
                }

                //if (isActive == false)
                //{
                //    hex_go.hole = false;
                //}

                hex_go.transform.SetParent(map.transform);
                hexes.Add(hex_go);
            }
        }
       
        return map;
    }

    public void MooveHexes()
    {
        if (Controller.Instance.gameState == GameState.doPlay)
        {
           
            changeTime = changeTime - 1 * Time.deltaTime;
            if (changeTime <= 0)
            {
                for (int i = 0; i < hexes.Count; i++)
                {
                    if (hexes[i].permission && hexes[i].end)
                    {
                        //int destiny = Random.Range(0, 100);

                        //if (holesNomber > 0 && destiny % 5 == 0)
                        //{
                        //    hexes[i].Move(minusePoints);
                        //    holesNomber--;
                        //}
                        //else
                        {
                            hexes[i].Move(points);
                        }
                    }
                }

                var list = hexes.Where(x => x.state == HexState.NONE).ToList();

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

                    

                changeTime = HUD.Instance.changesTime.value;
            }
        }
    }
}
