using System;
using Resources;
using Snails;
using UnityEngine;

namespace Buildings
{
    public class Cook : Storage
    {

        [Header("Cook")]
        [SerializeField] private Inventory drinkInventory;
        [SerializeField] private Inventory foodInventory;
        
        public int WaterAmount => drinkInventory.GetOutputAmount();
        public int FoodAmount => foodInventory.GetOutputAmount();
        
        public void ConsumeDrink(int amount)
        {
            drinkInventory.RemoveOutput(amount);
        }
        
        public void ConsumeFood(int amount)
        {
            foodInventory.RemoveOutput(amount);
        }

        public override void InsertResource(ResourceType resourceType, int amount)
        {
            if (resourceType == ResourceType.Food)
            {
                foodInventory.AddOutput(amount);
            }

            if (resourceType == ResourceType.Drink)
            {
                print("Drink");
                drinkInventory.AddOutput(amount);
            }
        }

        public override int GetRemainingSpace(ResourceType resourceType)
        {
            if(resourceType == ResourceType.Food)
            {
                return foodInventory.GetOutputSpace();
            }
            
            return drinkInventory.GetOutputSpace();
        }
        
        public int GetOutputAmount(ResourceType resourceType)
        {
            if(resourceType == ResourceType.Food)
            {
                return foodInventory.GetOutputAmount();
            }
            
            return drinkInventory.GetOutputAmount();
        }

        public override BuildingInfo GetBuildingInfo()
        {
            if (constructionSite != null) return constructionSite.GetConstructionInfo(BuildingData);
            
            Snail[] snails = Array.Empty<Snail>();
            return new BuildingInfo(this, BuildingData, snails, maxWorkerCount, drinkInventory.GetOutputSlot(),
                foodInventory.GetOutputSlot());
        }
    }
}
