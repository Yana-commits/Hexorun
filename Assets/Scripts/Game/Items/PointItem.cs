using MoreMountains.NiceVibrations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class PointItem : MonoBehaviour, ICollectable
{
    [SerializeField]
    private GameObject explosionPrefab;

    public void Collect(GameState gameState)
    {
        gameState.PointsAmount += 2;
        Instantiate(explosionPrefab, gameObject.transform.position, Quaternion.identity);
        Destroy(gameObject);
    }
}