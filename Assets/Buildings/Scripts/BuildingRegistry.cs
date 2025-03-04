using System;
using System.Collections.Generic;
using Resources;
using UnityEngine;

namespace Buildings
{
    public class BuildingRegistry : MonoBehaviour
    {
        public static BuildingRegistry Instance { get; private set; }
        
        [Header("Dont touch")]
        [SerializeField] private List<NeedsSatisfyingBuilding> needsSatisfyingBuildings= new List<NeedsSatisfyingBuilding>();
        [SerializeField] private List<Storage> storages = new List<Storage>();
        [SerializeField] private List<Building> buildings = new List<Building>();
        [SerializeField] private List<Inventory> inventories = new List<Inventory>();

        public Action<House> NewHouseBuild;
        
        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(this);
            }
            else
            {
                Instance = this;
            }
        }
        
        public void AddBuilding(Building building, BuildingType buildingType)
        {
            if (buildings.Contains(building)) return;
            
            buildings.Add(building);

            var buildingInventories = building.gameObject.GetComponents<Inventory>();

            foreach (var inventory in buildingInventories)
            {
                inventories.Add(inventory);
            }

            switch (buildingType)
            {
                case BuildingType.House:
                {
                    NewHouseBuild?.Invoke(building.gameObject.GetComponent<House>());
                    break;
                }
                case BuildingType.Storage:
                {
                    if (building.gameObject.TryGetComponent<Storage>(out var storage))
                    {
                        storages.Add(storage);
                    }
                    else throw new Exception("Storage not found on Storage building " + building.gameObject);
                    
                    break;
                }
                default:
                    break;
            }
        }

        public void RemoveBuilding(Building building, BuildingType buildingType)
        {
            if (!buildings.Contains(building)) return;
            
            buildings.Remove(building);

            if (building.gameObject.TryGetComponent<Inventory>(out var inventory))
            {
                if(inventories.Contains(inventory))
                {
                    inventories.Remove(inventory);
                }
            }

            switch (buildingType)
            {
                case BuildingType.House:
                {
                    if (building.gameObject.TryGetComponent<NeedsSatisfyingBuilding>(out var needsSatisfyingBuilding))
                    {
                        needsSatisfyingBuildings.Remove(needsSatisfyingBuilding);
                    }
                    break;
                }
                case BuildingType.Storage:
                {
                    if (building.gameObject.TryGetComponent<Storage>(out var storage))
                    {
                        storages.Remove(storage);
                    }
                    break;
                }
                default:
                    break;
            }
        }

        public bool TryGetHouse(out House house)
        {
            foreach (var building in buildings)
            {
                if (building.TryGetComponent<House>(out var tHouse) && tHouse)
                {
                    if(tHouse.HasSpace())
                    {
                        house = tHouse;
                        return true;
                    }
                }
            }

            house = null;
            return false;
        }

        public Storage GetNearestStorageOfType(ResourceType type, Vector3 position)
        {
            Storage nearestStorage = null;
            float nearestDistance = float.MaxValue;
            
            foreach (var storage in storages)
            {
                if (storage.gameObject.TryGetComponent<Cook>(out var cook))
                {
                    if(type != ResourceType.Food && type != ResourceType.Drink) continue;
                }
                else if ((storage.ResourceType != type) || (storage.GetRemainingSpace(type) <= 0))
                {
                    continue;
                }
                
                
                float distance = Vector3.Distance(storage.GetSnailTargetPosition(), position);
                if (distance < nearestDistance)
                {
                    nearestDistance = distance;
                    nearestStorage = storage;
                }
            }
            return nearestStorage;
        }
        
        
        public Inventory GetNearestOutputInventoryOfType(ResourceType type, Vector3 position)
        {
            Inventory nearestInventory = null;
            float nearestDistance = float.MaxValue;
            
            foreach (var iinventory in inventories)
            {
                if ((iinventory.OutPutResourceType != type) || (iinventory.GetOutputAmount() <= 0)) continue;
                
                float distance = Vector3.Distance(iinventory.transform.position, position);
                if (distance < nearestDistance)
                {
                    nearestDistance = distance;
                    nearestInventory = iinventory;
                }
            }
            return nearestInventory;
        }
    }
}
