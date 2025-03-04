using Drought;
using Resources;
using Snails;

namespace Buildings
{
    public class WaterPump : ProductionBuilding
    {
        public override void Update()
        {
            if (Status == BuildingStatus.Hologram) return;
            
            if (CanProduce())
            {
                Produce();
            }
            if(workers.Count > 0 && inventory.GetOutputAmount() > 0)
            {
                EmptyOutput();
            }
        }

        public override void AddSnail(SnailBehavior snail)
        {
            base.AddSnail(snail);
            snail.snail.inventorySlot.resourceType = ResourceType.Drink;
        }

        protected override bool CanProduce()
        {
            if (DroughtController.IsDrought) return false;
            if (workers.Count <= 0) return false;
            if (inventory.GetOutputSpace() <= 0) return false;
            
            return true;
        }
    }
}
