using System;
using System.Collections.Generic;
using System.Linq;
using Resources;
using Snails;
using UnityEngine;

namespace Buildings
{
    public class ConstructionSite : MonoBehaviour
    {
        private Building assignedBuilding;
        private TownHall townHall;

        private InventorySlot[] slots = Array.Empty<InventorySlot>();
        private int[] neededAmounts = Array.Empty<int>();

        public void Initialize(List<BuildingCost> costs, Building building, TownHall townHall)
        {
            assignedBuilding = building;
            this.townHall = townHall;
            
            townHall.AddConstructionSite(this);

            slots = new InventorySlot[costs.Count];
            neededAmounts = new int[costs.Count];
            
            for (int i = 0; i < costs.Count; i++)
            {
                slots[i] = new InventorySlot(costs[i].resourceType, costs[i].amount);
                neededAmounts[i] = costs[i].amount;
            }
        }

        public void SnailReached(ConstructionSnailBehavior snailBehavior)
        {
            print("[ConstructionSite] SnailReached");
            
            Snail snail = snailBehavior.snail;
            
            for (int i = 0; i < slots.Length; i++)
            {
                if(slots[i].resourceType == ResourceType.None || slots[i].RemainingSpace <= 0) continue;
                
                if (slots[i].resourceType == snail.inventorySlot.resourceType)
                {
                    slots[i].currentAmount += snail.inventorySlot.currentAmount;
                    snail.inventorySlot.currentAmount = 0;
                    break;
                }
            }
            
            if(!neededAmounts.Any(neededAmount => neededAmount > 0))
            {
                Build();
            }
            
            snailBehavior.GetNextTask();
        }

        public Vector3 GetSnailTargetPosition()
        {
            return assignedBuilding.GetSnailTargetPosition();
        }

        private void Build()
        {
            FinishBuild();
        }

        private void FinishBuild()
        {
            townHall.RemoveConstructionSite(this);
            assignedBuilding.OnConstructed();
            Destroy(gameObject);
        }

        public bool HasTask()
        {
            for (int i = 0; i < slots.Length; i++)
            {
                if (slots[i].currentAmount >= slots[i].maxAmount) continue;
                
                Inventory inventory = BuildingRegistry.Instance.GetNearestOutputInventoryOfType(slots[i].resourceType, transform.position);
                
                if (inventory == null) continue;
                if (inventory != null && inventory.GetOutputAmount() > 0) return true;
            }

            foreach (var slot in slots)
            {
                if(slot.RemainingSpace <= 0) continue;
                
                return false;
            }

            return true;
        }

        public void GetTask(ConstructionSnailBehavior snailBehavior)
        {
            for (int i = 0; i < slots.Length; i++)
            {
                if (neededAmounts[i] <= 0) continue;
                
                Inventory inventory = BuildingRegistry.Instance.GetNearestOutputInventoryOfType(slots[i].resourceType, transform.position);
                
                if (inventory == null) continue;
                
                snailBehavior.snail.inventorySlot.resourceType = slots[0].resourceType;
                int amount = Utils.Math.MinI(neededAmounts[i], snailBehavior.snail.inventorySlot.maxAmount, inventory.GetOutputAmount());

                neededAmounts[i] -= amount;
                snailBehavior.GetResources(slots[i].resourceType, amount, inventory);
                return;
            }
            
            if(!neededAmounts.Any(neededAmount => neededAmount > 0))
            {
                snailBehavior.snail.SetDestination(GetSnailTargetPosition());
                return;
            }
            
            snailBehavior.snail.SetDestination(GetSnailTargetPosition());
        }

        public void Demolish()
        {
            townHall.RemoveConstructionSite(this);
            Destroy(gameObject);
        }

        public BuildingInfo GetConstructionInfo(BuildingData data)
        {
          
            InventorySlot secondSlot = new InventorySlot(ResourceType.None, 0);
            if(slots.Length > 1) secondSlot = slots[1];
            
            return new BuildingInfo(assignedBuilding, data, new Snail[] { }, 0, slots[0], secondSlot, -1);
        }
    }
}
