using System;
using UnityEngine;

namespace Resources
{
    [RequireComponent(typeof(BoxCollider))]
    public class Resource : MonoBehaviour, IHarvestable
    {
        [SerializeField] protected int maxAmount = 10;
        protected int remainingAmount = 0;

        public bool TargetedBySnail = false;
        
        [SerializeField] private ResourceType resourceType;
        public ResourceType ResourceType { get; protected set; }

        public Action Harvested;

        private void Awake()
        {
            ResourceType = resourceType;
        }

        protected virtual void Start()
        {
            remainingAmount = maxAmount;
        }
        
        public virtual int Harvest(int amount)
        {
            if (amount < remainingAmount)
            {
                remainingAmount -= amount;
                return amount;
            }
            else
            {
                int amountHarvested = remainingAmount;
                remainingAmount = 0;
                Harvested?.Invoke();
                Destroy(gameObject, Time.deltaTime);
                return amountHarvested;
            }
        }
    }
}
