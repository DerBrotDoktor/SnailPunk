using System;
using System.Collections.Generic;
using Resources;
using UnityEngine;

namespace Buildings
{
    public class Inventory : MonoBehaviour
    {
        [SerializeField] private InventorySlot outputSlot;
        [SerializeField] private List<InventorySlot> inputSlots= new List<InventorySlot>();
        
        public ResourceType OutPutResourceType => outputSlot.resourceType;

        public int GetOutputSpace()
        {
            return outputSlot.RemainingSpace;
        }

        public int GetOutputAmount()
        {
            return outputSlot.currentAmount;
        }

        public int GetMaxOutput()
        {
            return outputSlot.maxAmount;
        }
        
        public void AddOutput(int amount)
        {
            outputSlot.currentAmount += amount;
            GlobalInventory.Instance.AddResource(outputSlot.resourceType,amount);
        }

        public int RemoveOutput(int amount)
        {
            int removedAmount = Math.Min(amount, outputSlot.currentAmount);
            outputSlot.currentAmount -= removedAmount;
            GlobalInventory.Instance.RemoveResource(outputSlot.resourceType,amount);
            return removedAmount;
        }
        
        public void AddInput(ResourceType resourceType, int amount)
        {
            for (int i = 0; i < inputSlots.Count; i++)
            {
                if (inputSlots[i].resourceType == resourceType)
                {
                    var slot = inputSlots[i];
                    slot.currentAmount += amount;
                    inputSlots[i] = slot;
                    
                    GlobalInventory.Instance.AddResource(resourceType, amount);
                    return;
                }
            }
        }
    
        public void RemoveInput(ResourceType resourceType, int amount)
        {
            if(resourceType == ResourceType.None) return;
            
            for (int i = 0; i < inputSlots.Count; i++)
            {
                if (inputSlots[i].resourceType == resourceType)
                {
                    var slot = inputSlots[i];
                    slot.currentAmount -= amount;
                    inputSlots[i] = slot;
                    
                    GlobalInventory.Instance.RemoveResource(resourceType, amount);
                    return;
                }
            }
        }
        public bool TryInputResource(ResourceType resourceType, int amount, out int insertedAmount)
        {
            for (int i = 0; i < inputSlots.Count; i++)
            {
                if (inputSlots[i].resourceType == resourceType)
                {
                    if (inputSlots[i].RemainingSpace > 0)
                    {
                        var slot = inputSlots[i];
                        
                        insertedAmount = Mathf.Min(slot.RemainingSpace, amount);
                        
                        slot.currentAmount += insertedAmount;
                        inputSlots[i] = slot;
                        
                        GlobalInventory.Instance.AddResource(resourceType, amount);
                        return true;
                    }
                }
            }
            insertedAmount = 0;
            return false;
        }
        
        public int GetMaxInput(ResourceType resourceType)
        {
            for (int i = 0; i < inputSlots.Count; i++)
            {
                if (inputSlots[i].resourceType == resourceType)
                {
                    return inputSlots[i].maxAmount;
                }
            }
            return 0;
        }
        
        public int GetInputAmount(ResourceType resourceType)
        {
            for (int i = 0; i < inputSlots.Count; i++)
            {
                if (inputSlots[i].resourceType == resourceType)
                {
                    return inputSlots[i].currentAmount;
                }
            }
            return 0;
        }
        
        public List<ResourceType> GetInputResourceTypes()
        {
            List<ResourceType> resourceTypes = new List<ResourceType>();
            for (int i = 0; i < inputSlots.Count; i++)
            {
                resourceTypes.Add(inputSlots[i].resourceType);
            }
            return resourceTypes;
        }

        public InventorySlot GetOutputSlot()
        {
            return outputSlot;
        }
        
        public InventorySlot[] GetInputSlots()
        {
            return inputSlots.ToArray();
        }

        private void OnDestroy()
        {
            GlobalInventory.Instance.RemoveResource(OutPutResourceType, GetOutputAmount());
            for (int i = 0; i < inputSlots.Count; i++)
            {
                GlobalInventory.Instance.RemoveResource(inputSlots[i].resourceType, inputSlots[i].currentAmount);
            }
        }
    }
}
