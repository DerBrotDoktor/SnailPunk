using System;
using System.Collections.Generic;
using Resources;
using Snails;

namespace Buildings
{
    public class House : Building
    {
        public Action ADemolish;

        public int MaxCapacity { get; } = 4;
        private List<Snail> _snails = new List<Snail>();

        public override void OnConstructed()
        {
            base.OnConstructed();
            GlobalInventory.Instance.AddResource(ResourceType.HouseSpace, MaxCapacity);
        }

        public override void DestroyBuilding()
        {
            GlobalInventory.Instance.RemoveResource(ResourceType.HouseSpace, MaxCapacity);
            base.DestroyBuilding();
            ADemolish?.Invoke();
            Demolish();
        }
        
        public bool TryAddSnail(Snail snail)
        {
            if (!HasSpace()) return false;
            _snails.Add(snail);
            return true;
        }
        
        public void RemoveSnail(Snail snail)
        {
            if (_snails.Contains(snail))
            {
                _snails.Remove(snail);
            }
        }
        
        public bool HasSpace()
        {
            return _snails.Count < MaxCapacity;
        }

        public override BuildingInfo GetBuildingInfo()
        {
            if (constructionSite != null) return constructionSite.GetConstructionInfo(BuildingData);
            
            return new BuildingInfo(this, BuildingData, _snails.ToArray(), MaxCapacity);
        }
    }
}
