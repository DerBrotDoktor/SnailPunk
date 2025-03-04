using UnityEngine;
using FMODUnity;
using System.Collections.Generic;

public class FMODChildObjectMonitoring : MonoBehaviour
{
    public EventReference destroyEvent; // The FMOD event to play when the child is destroyed

    public string targetChildName; // The specific name of the child object to monitor

    private List<Transform> previousChildren = new List<Transform>();

    private void Start()
    {
        InitializeChildren();
    }

    private void InitializeChildren()
    {
        // Initialize the list with the current children
        foreach (Transform child in transform)
        {
            previousChildren.Add(child);
        }
    }

    private void OnTransformChildrenChanged()
    {
        List<Transform> currentChildren = new List<Transform>();
        foreach (Transform child in transform)
        {
            currentChildren.Add(child);
        }

        // Check for removed specific child
        foreach (Transform child in previousChildren)
        {
            if (!currentChildren.Contains(child) && child.name.StartsWith(targetChildName))
            {
                Debug.Log("Target child removed: " + child.name);
                OnPrefabDestroyed();
            }
        }

        // Update the previousChildren list
        previousChildren = currentChildren;
    }

    private void OnPrefabDestroyed()
    {
        // Use the parent's position for the FMOD event
        Vector3 parentPosition = transform.position;
        RuntimeManager.PlayOneShot(destroyEvent, parentPosition);
    }
}
