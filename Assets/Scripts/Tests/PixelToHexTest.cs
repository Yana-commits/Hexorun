using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class PixelToHexTest : MonoBehaviour
{
    [SerializeField] Map map;

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        var hex = map.GetHexByPosition(transform.position);
        var pos = hex.transform.position;

        Gizmos.color = Color.green;
        Gizmos.DrawSphere(pos, Map.hexRadius/2);

        var neigh = Hexagonal.Offset.GetQNeighbour(hex.index)
            .Where(d => (d - hex.index).y >= (hex.index.x & 1))
            .Select(n => map[n].transform.position);

        Gizmos.color = Color.red;
        foreach (var item in neigh)
        {
            Gizmos.DrawSphere(item, Map.hexRadius / 2);
        }

    }
#endif
}
