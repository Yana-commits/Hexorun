using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform player;
    public float smooth = 5.0f;
    public Vector3 offset = new Vector3(0, 2, 0);
    public float MinX, MaxX, MinZ, MaxZ;


    void Update()
    {
        Vector3 target = new Vector3
             (
            Mathf.Clamp(player.position.x - offset.x, MinX, MaxX),
            offset.y,
            Mathf.Clamp(player.position.z - offset.z, MinZ, MaxZ)
            );

        transform.position = Vector3.Lerp(transform.position, target, Time.deltaTime * smooth);
    }
}
