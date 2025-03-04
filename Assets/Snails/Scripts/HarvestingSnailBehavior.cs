using System.Collections;
using Buildings;
using Plants;
using UnityEngine;

namespace Snails
{
    public class HarvestingSnailBehavior : SnailBehavior
    {
        [SerializeField] private int harvestAmount = 1;
        [SerializeField] private float timeToHarvest = 1f;
        public Harvester Harvester;
        
        private Plant TargetResource;
        private bool isHarvesting = false;
        private bool walkToResource = false;

        public void SetTargetResource(Plant resource)
        {
            if(resource == null) return;

            if (!resource.TrySetTarget()) return;
            
            TargetResource = resource;
            walkToResource = true;
            snail.SetDestination(resource.transform.position,true);
        }
        
        private Storage targetStorage;

        public void SetTargetStorage(Storage storage)
        {
            targetStorage = storage;
            snail.SetDestination(storage.GetSnailTargetPosition());
        }
        
        public override void OnTargetReached()
        {
            if (isHarvesting) return;
            
            if (walkToResource)
            {
                if (TargetResource == null)
                {
                    snail.SetDestination(assignedBuilding.InternalStreetPosition, true);
                    walkToResource = false;
                    return;
                }
                
                StartHarvesting();
            }
            else if (targetStorage != null)
            {
                targetStorage.InsertResource(snail.inventorySlot.resourceType, snail.inventorySlot.currentAmount);
                snail.inventorySlot.currentAmount = 0;
                targetStorage = null;
                NextTask();
            }
            else
            {
                snail.NextTask();
            }
        }

        private void StartHarvesting()
        {
            isHarvesting = true;
            walkToResource = false;
            StartCoroutine(Harvest());
        }
        
        private IEnumerator Harvest()
        {
            float harvestTime = TargetResource.GetHarvestTime();
            yield return new WaitForSeconds(harvestTime);
            
            if (TargetResource != null)
            {
                snail.inventorySlot.currentAmount += TargetResource.Harvest();
            }
            
            isHarvesting = false;

            snail.SetDestination(assignedBuilding.GetSnailTargetPosition(), true);
        }

        protected override void SnailReached()
        {
            Debug.Log("[HarvestingSnailBehavior] Snail reached");
            
            if (Harvester == null)
            {
               Harvester = assignedBuilding.GetComponent<Harvester>();
            }
            
            Harvester.SnailReached(this);
        }
    }
}
