using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemsCollector : MonoBehaviour
{
    [SerializeField] GameState _gameState;

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<ICollectable>(out var collectable))
        {
            collectable.Collect(_gameState);
        }
    }
}
