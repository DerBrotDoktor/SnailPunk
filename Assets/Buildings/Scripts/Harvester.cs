using System;
using System.Collections;
using System.Collections.Generic;
using BuildingSystem;
using Plants;
using Resources;
using Snails;
using UnityEngine;

namespace Buildings
{
    [RequireComponent(typeof(Inventory))]
    public class Harvester : Building
    {
        [SerializeField] private GameObject rangeCircle;
        [SerializeField] private float radius = 2f;
        [SerializeField] private ResourceType harvestResourceType;
        [SerializeField] private LayerMask resourceLayerMask;
        
        private Inventory inventory;

        private List<HarvestingSnailBehavior> snailQueue = new List<HarvestingSnailBehavior>();

        protected override void Awake()
        {
            base.Awake();
            inventory = GetComponent<Inventory>();
        }

        private Plant FindPlant()
        {
            Collider[] hitColliders = Physics.OverlapSphere(transform.position, radius, resourceLayerMask);
            foreach (var hitCollider in hitColliders)
            {
                if (hitCollider.gameObject.TryGetComponent<Plant>(out var plant))
                {
                    if (plant.ResourceType == harvestResourceType && !plant.IsTargeted() && plant.CanBeHarvested())
                    {
                        return plant;
                    }
                }
            }

            return null;
        }
        
        public override void OnBuild(Vector3Int colliderSize, Vector3 internalStreetPosition, BuildingData buildingData, bool construction = true)
        {
            base.OnBuild(colliderSize, internalStreetPosition, buildingData, construction);
            rangeCircle.SetActive(false);
            print("Build Harvester");
        }

        public override void OnHologram(Material hologramMaterial)
        {
            base.OnHologram(hologramMaterial);
            rangeCircle.transform.localScale = new Vector3(radius*2,radius*2,1);
            rangeCircle.SetActive(true);
        }
        
        public void SnailReached(HarvestingSnailBehavior snail)
        {
            if (snail.GetComponent<Snail>().inventorySlot.resourceType != harvestResourceType)
            {
                snail.GetComponent<Snail>().inventorySlot.resourceType = harvestResourceType;
            }

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
            if (snail.gameObject.TryGetComponent<HarvestingSnailBehavior>(out HarvestingSnailBehavior hSnail))
            {
                base.AddSnail(hSnail);
            }
        }

        public override void Update()
        {
            base.Update();
            
            if(snailQueue.Count <= 0) return;

            if (inventory.GetOutputSpace() > 0)
            {
                
                HarvestingSnailBehavior snail = snailQueue[0];
                int amount = Math.Min(inventory.GetOutputSpace(), snail.GetComponent<Snail>().inventorySlot.currentAmount);
                inventory.AddOutput(amount);
                snail.GetComponent<Snail>().inventorySlot.currentAmount -= amount;
                
                UpdateBuildingInfo?.Invoke(GetBuildingInfo());
                
                if(snail.GetComponent<Snail>().inventorySlot.currentAmount <= 0)
                {
                    Plant plant = FindPlant();
                    
                    if (plant == null) return;
                    
                    snail.SetTargetResource(plant);
                    snailQueue.Remove(snail);
                }
            }
            else
            {
                print("output full");
                Storage storage = BuildingRegistry.Instance.GetNearestStorageOfType(harvestResourceType, InternalStreetPosition);
                if(storage == null) return;
                
                int amount = Utils.Math.MinI(storage.GetRemainingSpace(harvestResourceType), inventory.GetOutputAmount(), snailQueue[0].GetComponent<Snail>().inventorySlot.RemainingSpace);
                
                snailQueue[0].GetComponent<Snail>().inventorySlot.currentAmount += amount;
                inventory.RemoveOutput(amount);

                UpdateBuildingInfo?.Invoke(GetBuildingInfo());
                
                snailQueue[0].SetTargetStorage(storage);
                snailQueue.RemoveAt(0);
            }
        }
        
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.magenta;
            Gizmos.DrawWireSphere(transform.position, radius);
        }

        public override void DestroyBuilding()
        {
            base.DestroyBuilding();
            // for (int i = 0; i < requestedWorkerCount; i++)
            // {
            //     DecreaseWorkerCount();
            // }
            //
            // SnailBehavior[] tSnails = new SnailBehavior[snails.Count];
            // snails.CopyTo(tSnails); 
            // foreach (var snail in tSnails)
            // {
            //     snail.shouldBeUnAssigned = true;
            //     if (snailQueue.Contains(snail.gameObject.GetComponent<HarvestingSnailBehavior>()))
            //     {
            //         SnailReached(snail);
            //     }
            // }
        }

        public override BuildingInfo GetBuildingInfo()
        {
            if (constructionSite != null) return constructionSite.GetConstructionInfo(BuildingData);
            
            Snail[] snails = new Snail[this.snails.Count];
            for (int i = 0; i < this.snails.Count; i++)
            {
                snails[i] = this.snails[i].snail;
            }
            
            return new BuildingInfo(this, BuildingData, snails, maxWorkerCount, inventory.GetOutputSlot());
        }
    }
}
