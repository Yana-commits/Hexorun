using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class ArenaShaderAssistant : MonoBehaviour
{
    [SerializeField] private Material globalMaterial;
    [SerializeField] private Transform targetTransform;
    
    private void Update()
    {
        var targetv = targetTransform.position;
        targetv.y = 0;
        
        globalMaterial.SetVector("Target", targetv);
    }
}