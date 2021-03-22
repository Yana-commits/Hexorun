using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PixelToHexTest : MonoBehaviour
{
    [SerializeField] Map map;

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        var pos = map.GetHexByPosition(transform.position).transform.position;

        Gizmos.color = Color.green;
        Gizmos.DrawSphere(pos, Map.hexRadius/2);

    }
#endif
}
