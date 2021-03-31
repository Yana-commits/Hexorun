using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Star : MonoBehaviour
{
    public delegate void CollectDelegate();
    public static event CollectDelegate Score;

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<Player>(out var player))
        {
            Handheld.Vibrate();
            Score();
            Destroy(gameObject);
        }
    }
}
