using System;
using BuildingSystem;
using UnityEngine;

public class ObjectDestroyEffect : MonoBehaviour
{
    [SerializeField] private Renderer[] meshRenderer = Array.Empty<Renderer>();
    
    [SerializeField] private int mouseLayer = 11;
    
    [SerializeField] private Material originalMaterial;
    [SerializeField] private Material destroyMaterial;
    
    private bool isDestroying = false;

    private void Update()
    {
        if(!isDestroying) return;
        if (!ObjectDestroyer.IsDestroying)
        {
            StopDestroy();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(gameObject.CompareTag("Hologram")) return;
        
        if (other.gameObject.layer == mouseLayer)
        {
            StartDestroy();
        }
    }
    
    private void OnTriggerExit(Collider other)
    {
        if(gameObject.CompareTag("Hologram")) return;
        if (other.gameObject.layer != mouseLayer) return;
        
        StopDestroy();
    }
    
    private void StartDestroy()
    {
        if(originalMaterial == null) originalMaterial = meshRenderer[0].material;
        
        isDestroying = true;
        foreach (var renderer in meshRenderer)
        {
            renderer.material = destroyMaterial;
        }
    }
    
    private void StopDestroy()
    {
        isDestroying = false;
        foreach (var renderer in meshRenderer)
        {
            renderer.material = originalMaterial;
        }
    }
}
