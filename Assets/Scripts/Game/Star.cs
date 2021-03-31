using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Star : MonoBehaviour
{
    public delegate void CollectDelegate();
    public static event CollectDelegate Score;
    [SerializeField]
    private GameObject explosionPrefab;

    [System.Obsolete]
    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<Player>(out var player))
        {
          var  newExplosion = Instantiate(explosionPrefab, gameObject.transform.position, Quaternion.identity) as GameObject;
            _ = newExplosion.GetComponent<ParticleSystem>().startLifetime;

            Handheld.Vibrate();
            Score();
            Destroy(gameObject);
        }
    }
}
