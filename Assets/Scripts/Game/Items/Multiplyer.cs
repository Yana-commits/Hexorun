using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(Collider))]
public class Multiplyer : MonoBehaviour, ICollectable
{
    public int n;

    public PlayerState state;

    [SerializeField]
    private GameObject explosionPrefab;
    public void Collect(GameState gameState)
    {
        Instantiate(explosionPrefab, gameObject.transform.position, Quaternion.identity);
  
        gameState.CoinAmount *= n;

        gameState.OnPlayerStateChanged(state);
        Destroy(gameObject);
    }
}
