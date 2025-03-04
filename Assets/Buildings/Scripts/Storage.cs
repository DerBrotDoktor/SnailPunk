using Resources;
using Snails;
using UnityEngine;

namespace Buildings
{
    [RequireComponent(typeof(Inventory))]
    public class Storage : Building
    {
        [Header("Storage")]
        [SerializeField] private ResourceType _resourceType;
        public ResourceType ResourceType => _resourceType;
        
         private Inventory inventory;

        protected override void Awake()
        {
            base.Awake();
            inventory = GetComponent<Inventory>();
        }

        public virtual void InsertResource(ResourceType resourceType, int amount)
        {
            inventory.AddOutput(amount);
        }

        public virtual int GetRemainingSpace(ResourceType resourceType)
        {
            return inventory.GetOutputSpace();
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
