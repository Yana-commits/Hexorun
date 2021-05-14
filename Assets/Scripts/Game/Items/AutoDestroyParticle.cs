using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoDestroyParticle : MonoBehaviour
{
    [SerializeField] ParticleSystem _particleSystem;
    [SerializeField] float duration;

    private void Awake()
    {
        if (_particleSystem)
        {
            duration = _particleSystem.main.duration +
                _particleSystem.main.startLifetime.constantMax +
                _particleSystem.main.startDelay.constant;
        }
    }

    private void Start()
    {
        Destroy(gameObject, duration);            
    }

}
