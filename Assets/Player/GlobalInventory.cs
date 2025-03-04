using System;
using System.Collections.Generic;
using BuildingSystem;
using Resources;
using UnityEngine;

public class GlobalInventory : MonoBehaviour
{
    public static GlobalInventory Instance { get; private set; }

    public static Action<ResourceType, int> ResourceChanged;
    
    private Dictionary<ResourceType, int> inventory = new Dictionary<ResourceType, int>();
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Debug.LogError("Global inventory already exists " + gameObject.name);
            Destroy(gameObject);
        }
    }
    
    public void AddResource(ResourceType resourceType, int amount)
    {
        
        if (!inventory.ContainsKey(resourceType))
        {
            inventory.Add(resourceType, amount);
        }
        else
        {
            inventory[resourceType] += amount;
        }
        
        ResourceChanged?.Invoke(resourceType, inventory[resourceType]);
    }
    
    public void RemoveResource(ResourceType resourceType, int amount)
    {
        if (inventory.ContainsKey(resourceType))
        {
            if(inventory[resourceType] - amount < 0)
            {
                inventory[resourceType] = 0;
            }
            else
            {
                inventory[resourceType] -= amount;
            }
            ResourceChanged?.Invoke(resourceType, inventory[resourceType]);
        }
    }
    
    public int GetResource(ResourceType resourceType)
    {
        if (inventory.ContainsKey(resourceType))
        {
            return inventory[resourceType];
        }

        return 0;
    }
}
