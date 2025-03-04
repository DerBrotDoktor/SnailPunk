using System;
using System.Collections.Generic;
using System.Linq;
using Plants;
using Resources;
using Snails;
using UnityEngine;
using UnityEngine.Serialization;

namespace Buildings
{
    [RequireComponent(typeof(Inventory))]
    public class Farm : Building
    {
        [SerializeField] private GameObject rangeCircle;
        [SerializeField] private float radius = 2f;
        [SerializeField] private InventorySlot snailInventorySlotPrefab;
        
        [SerializeField] private ResourceType[] targetResourceTypes;
        [SerializeField] private LayerMask plantLayerMask;
        [SerializeField] private LayerMask fieldLayerMask;
        
        private Inventory inventory;

        private List<FarmSnailBehavior> snailQueue = new List<FarmSnailBehavior>();

        protected override void Awake()
        {
            base.Awake();
            inventory = GetComponent<Inventory>();
        }

        private Plant FindNextPlant()
        {
            float minDistance = float.MaxValue;
            Plant closestPlant = null;
            
            Collider[] hitColliders = Physics.OverlapSphere(transform.position, radius, plantLayerMask);
            foreach (var hitCollider in hitColliders)
            {
                if (hitCollider.gameObject.TryGetComponent<Plant>(out var plant))
                {
                    if (targetResourceTypes.Contains(plant.ResourceType) && !plant.IsTargeted() && plant.CanBeHarvested())
                    {
                        float distance = Vector3.Distance(transform.position, plant.transform.position);
                    
                        if (distance <= radius && distance < minDistance)
                        {
                            minDistance = distance;
                            closestPlant = plant;
                        }
                    }
                }
            }

            return closestPlant;
        }
        
        private Field FindNextField()
        {
            float minDistance = float.MaxValue;
            Field closestField = null;
            
            Collider[] hitColliders = Physics.OverlapSphere(transform.position, radius, fieldLayerMask);
            foreach (var hitCollider in hitColliders)
            {
                if (!hitCollider.gameObject.TryGetComponent<Field>(out var field) || field.IsSnailTarget || !field.CanBePlanted()) continue;
                if (!targetResourceTypes.Contains(field.ResourceType)) continue;
                
                float distance = Vector3.Distance(transform.position, field.transform.position);
                    
                if (distance <= radius && distance < minDistance)
                {
                    minDistance = distance;
                    closestField = field;
                }
            }
            return closestField;
        }
        
        public override void OnBuild(Vector3Int colliderSize, Vector3 internalStreetPosition, BuildingData buildingData, bool construction = true)
        {
            base.OnBuild(colliderSize, internalStreetPosition, buildingData, construction);
            rangeCircle.SetActive(false);
            print("Build Farm");
        }

        public override void OnHologram(Material hologramMaterial)
        {
            base.OnHologram(hologramMaterial);
            rangeCircle.transform.localScale = new Vector3(radius*2,radius*2,1);
            rangeCircle.SetActive(true);
        }
        
        public void SnailReached(FarmSnailBehavior snail)
        {
            if (snail.shouldBeUnAssigned)
            {
                snails.Remove(snail);
                townHall.UnAssignSnail(snail.gameObject.GetComponent<Snail>());
                if (snails.Count <= 0)
                {
                    Demolish();
                }
                
                return;
            }
            snailQueue.Add(snail);
        }

        public override void AddSnail(SnailBehavior snail)
        {
            if (snail.gameObject.TryGetComponent<FarmSnailBehavior>(out FarmSnailBehavior farmSnail))
            {
                base.AddSnail(farmSnail);
                farmSnail.snail.inventorySlot.resourceType = ResourceType.Food;
            }
        }

        public override void Update()
        {
            base.Update();
        
            FullOutput();
            HarvestPlant();
            PlantField();
            
        }

        private void PlantField()
        {
            if (snailQueue.Count <= 0) return;
        
            Field field = FindNextField();

            if (field != null)
            {
                FarmSnailBehavior snail = snailQueue[0];
                snail.SetTargetField(field);
                snailQueue.RemoveAt(0);
            }
        }

        private void HarvestPlant()
        {
            if (snailQueue.Count <= 0 || inventory.GetOutputSpace() <= 0) return;
            
            FarmSnailBehavior snail = snailQueue[0];
            
            int amount = Math.Min(inventory.GetOutputSpace(), snail.GetComponent<Snail>().inventorySlot.currentAmount);
            inventory.AddOutput(amount);
            snail.GetComponent<Snail>().inventorySlot.currentAmount -= amount;
            
            UpdateBuildingInfo?.Invoke(GetBuildingInfo());
            
            if(snail.GetComponent<Snail>().inventorySlot.currentAmount <= 0)
            {
                Plant plant = FindNextPlant();
                
                if (plant == null) return;
                
                snail.SetTargetPlant(plant);
                snailQueue.Remove(snail);
            }
        }

        private void FullOutput()
        {
            if(snailQueue.Count <= 0 || inventory.GetOutputAmount() <= 0) return;
            
            Storage storage = BuildingRegistry.Instance.GetNearestStorageOfType(ResourceType.Food, InternalStreetPosition);
            if(storage == null) return;
            
            int amount = Utils.Math.MinI(storage.GetRemainingSpace(ResourceType.Food), inventory.GetOutputAmount(), snailQueue[0].GetComponent<Snail>().inventorySlot.RemainingSpace);
            
            snailQueue[0].GetComponent<Snail>().inventorySlot.currentAmount += amount;
            inventory.RemoveOutput(amount);

            UpdateBuildingInfo?.Invoke(GetBuildingInfo());
            
            snailQueue[0].SetTargetStorage(storage);
            snailQueue.RemoveAt(0);
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position, radius);
        }

        public override BuildingInfo GetBuildingInfo()
        {
            if(constructionSite != null) return constructionSite.GetConstructionInfo(BuildingData);
            
            Snail[] snails = new Snail[this.snails.Count];
            for (int i = 0; i < this.snails.Count; i++)
            {
                snails[i] = this.snails[i].snail;
            }
            
            return new BuildingInfo(this, BuildingData, snails, maxWorkerCount, inventory.GetOutputSlot());
        }
    }
}
