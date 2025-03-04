using Buildings;
using Resources;
using UnityEngine;

namespace Snails
{
    public class ConstructionSnailBehavior : SnailBehavior
    {
        [SerializeField] private ConstructionSite assignedConstructionSite;
        public ConstructionSite AssignedConstructionSite => assignedConstructionSite;
        private bool getResources = false;

        private Inventory targetInventory;
        private int targetAmount;

        public bool IsConstructionSiteAssigned => assignedConstructionSite != null;

        public void AssignConstructionSite(ConstructionSite constructionSite)
        {
            assignedConstructionSite = constructionSite;
            constructionSite.GetTask(this);
        }

        public void GetResources(ResourceType resourceType, int amount, Inventory inventory)
        {
            getResources = true;

            targetInventory = inventory;
            targetAmount = amount;
            snail.inventorySlot.resourceType = resourceType;

            Debug.Log("SetAmount " + targetAmount + " " + amount);

            Vector3 targetPosition = inventory.gameObject.GetComponent<Building>().GetSnailTargetPosition();
            snail.SetDestination(targetPosition);
        }

        protected override void SnailReached()
        {
            Debug.Log("[ConstructionSnailBehavior] SnailReached");

            if (!IsConstructionSiteAssigned)
            {
                UnAssignConstructionSite();
                GetNextTask();
                return;
            }

            if (getResources)
            {
                HandleResourceCollection();
            }
            else
            {
                assignedConstructionSite.SnailReached(this);
            }
        }

        private void HandleResourceCollection()
        {
            getResources = false;

            if (targetInventory != null)
            {
                snail.inventorySlot.currentAmount = targetInventory.RemoveOutput(targetAmount);

                Vector3 constructionSitePosition = assignedConstructionSite.GetSnailTargetPosition();
                snail.SetDestination(constructionSitePosition);
            }

            targetInventory = null;
            targetAmount = 0;
        }

        public void UnAssignConstructionSite()
        {
            assignedConstructionSite = null;
        }

        public void GetNextTask()
        {
            Debug.Log("[ConstructionSnailBehavior] GetNextTask");
            TownHall townHall = FindObjectOfType<TownHall>();

            if (townHall != null)
            {
                townHall.GetNextTask(this);
            }
        }
    }
}
