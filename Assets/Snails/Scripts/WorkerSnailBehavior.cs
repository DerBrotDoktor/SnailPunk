using System;
using Buildings;
using Resources;
using UnityEngine;

namespace Snails
{
    public class WorkerSnailBehavior : SnailBehavior
    {
        
        bool isInAssignedBuilding = false;
        private ProductionBuilding productionBuilding;
        private TownHall townHall;
        
        private bool getResources = false;
        private Inventory targetInventory;
        private Storage targetStorage;
        
        protected override void Awake()
        {
            base.Awake();
            townHall = FindObjectOfType<TownHall>();
        }

        public override void SetAssignedBuilding(Building building)
        {
            base.SetAssignedBuilding(building);
            productionBuilding = assignedBuilding.gameObject.GetComponent<ProductionBuilding>();
        }
        
        public void GetResource(ResourceType resourceType, Inventory targetInventory)
        {
            this.targetInventory = targetInventory;
            snail.inventorySlot.resourceType = resourceType;
            
            getResources = true;
            
            snail.SetDestination(targetInventory.gameObject.GetComponent<Building>().InternalStreetPosition);
        }
        
        public void SetTargetStorage(Storage storage)
        {
            targetStorage = storage;
            snail.SetDestination(storage.GetSnailTargetPosition());
        }
        
        public override void OnTargetReached()
        {
            if (snail.ShouldSatisfyNeeds)
            {
                snail.NextTask();
                return;
            }
            
            if (targetInventory != null)
            {
                int amount = Math.Min(snail.inventorySlot.maxAmount, productionBuilding.RemainingInput);
                amount = Math.Min(amount, targetInventory.GetOutputAmount());
                
                targetInventory.RemoveOutput(amount);
                snail.inventorySlot.currentAmount += amount;
                
                targetInventory = null;
                
                snail.SetDestination(assignedBuilding.InternalStreetPosition);
                return;
            }
            else if (targetStorage != null)
            {            
                print("[WorkerSnailBehavior] targetStorage Reached " + targetStorage + " " + snail.inventorySlot.resourceType + " " + snail.inventorySlot.currentAmount);
                targetStorage.InsertResource(snail.inventorySlot.resourceType, snail.inventorySlot.currentAmount);
                snail.inventorySlot.currentAmount = 0;
                
                snail.SetDestination(assignedBuilding.InternalStreetPosition);
                
                targetStorage = null;
                return;
            }
            else if (getResources)
            {
                productionBuilding.AddResources(snail.inventorySlot.currentAmount);
                snail.inventorySlot.currentAmount = 0;
                getResources = false;
            }
            
            snail.NextTask();
        }
    }
}
