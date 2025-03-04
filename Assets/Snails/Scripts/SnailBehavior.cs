using Buildings;
using UnityEngine;

namespace Snails
{
    public class SnailBehavior : MonoBehaviour
    {
        public bool IsAssigned = false;
        public bool shouldBeUnAssigned = false;
        
        public Building assignedBuilding;
        private BuildingRegistry buildingRegistry;

        public Snail snail { get; private set; }

        protected virtual void Awake()
        {
            snail = GetComponent<Snail>();
        }

        public virtual void SetAssignedBuilding(Building building)
        {
            if (assignedBuilding != null)
            {
                assignedBuilding.RemoveSnail(this);
            }
            
            assignedBuilding = building;
            building?.AddSnail(this);
        }

        public virtual void OnTargetReached()
        {
            snail.NextTask();
        }

        public void NextTask()
        {
            SnailReached();
        }

        protected virtual void SnailReached()
        {
            assignedBuilding?.SnailReached(this);
        }

        public void ForceUnAssign()
        {
            shouldBeUnAssigned = true;
            
            snail.inventorySlot.currentAmount = 0;
            
            if (!snail.SatisfyingNeeds)
            {
                snail.SetDestination(FindObjectOfType<TownHall>().GetSnailTargetPosition(), true);
            }
            
            FindObjectOfType<TownHall>().UnAssignSnail(snail);
        }
    }
}
