using MoreMountains.NiceVibrations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Star : MonoBehaviour, ICollectable
{
    [SerializeField]
    private GameObject explosionPrefab;

    public void Collect(GameState gameState)
    {
        gameState.CoinAmount++;
        Instantiate(explosionPrefab, gameObject.transform.position, Quaternion.identity);
        MMVibrationManager.Haptic(HapticTypes.Success);
        Destroy(gameObject);
    }
}
