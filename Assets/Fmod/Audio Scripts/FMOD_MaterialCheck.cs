using UnityEngine;
using FMODUnity;
using Sound;

public class FMOD_MaterialCheck : MonoBehaviour
{
    // The material you want to check for
    public Material specificMaterial;

    [Header("Sound")]
    [SerializeField] private SoundEmitter soundEmitter;


    // Flag to ensure the event is only triggered once
    private bool eventTriggered = false;

    private void Update()
    {
        // Continuously check if any child object has the specific material and is enabled
        if (!eventTriggered && CheckMaterialInChildren(transform))
        {
            // Trigger the FMOD event
            soundEmitter.Play();
            eventTriggered = true; // Prevent the event from triggering multiple times
        }
    }

    private bool CheckMaterialInChildren(Transform parentTransform)
    {
        // Loop through each child object (including the parent itself)
        foreach (Transform child in parentTransform)
        {
            // Check if the child object is active
            if (child.gameObject.activeInHierarchy)
            {
                // Get the MeshRenderer component
                MeshRenderer meshRenderer = child.GetComponent<MeshRenderer>();

                // Check if the MeshRenderer exists
                if (meshRenderer != null)
                {
                    // Loop through all materials in the MeshRenderer
                    foreach (Material material in meshRenderer.materials)
                    {
                        // Check if the material name matches or if the materials are equal
                        if (material.name.StartsWith(specificMaterial.name) || material.Equals(specificMaterial))
                        {
                            return true; // Found the material and the object is active, return true
                        }
                    }
                }
            }

            // Recursively check child objects
            if (CheckMaterialInChildren(child))
            {
                return true;
            }
        }

        // No matching material found in an active child object
        return false;
    }
    private void OnDestroy()
    {
        soundEmitter.Stop();
    }
  

}
