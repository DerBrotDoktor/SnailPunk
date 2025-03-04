using System.Collections.Generic;
using Resources;
using Snails;
using UnityEngine;
using Utils;

namespace Buildings
{
    //TODO: Snail Inventory Slot
    
    [RequireComponent(typeof(Inventory))]
    public class ProductionBuilding : Building
    {
        [Header("ProductionBuilding")]
        [SerializeField] private float productionTime;
        private float currentProductionTime;

        [SerializeField] private ResourceType inputResource;
        public ResourceType InputResource => inputResource;
        [SerializeField] private int neededInput;
        [SerializeField] private ResourceType outputResource;
        [SerializeField] private int outputAmount = 1;

        protected Inventory inventory;
        public int RemainingInput => inventory.GetMaxInput(inputResource);
        
        [SerializeField] protected List<WorkerSnailBehavior> workers = new List<WorkerSnailBehavior>();
        
        private bool sendSnailGetResources = false;
        
        protected override void Awake()
        {
            base.Awake();
            inventory = GetComponent<Inventory>();
        }
        
        public override void Update()
        {
            base.Update();

            if (CanProduce())
            {
                Produce();
            }
            else if (workers.Count > 0 && inventory.GetOutputSpace() <= 0)
            {
                EmptyOutput();
            }
            else if (workers.Count > 0 && inventory.GetInputAmount(inputResource) < neededInput)
            {
                FillInput();
            }
        }

        protected void Produce()
        {
            if (currentProductionTime < productionTime)
            {
                currentProductionTime += Time.deltaTime * Game.TimeScale;
            }
            else
            {
                currentProductionTime = 0;
                inventory.AddOutput(outputAmount);
                inventory.RemoveInput(inputResource, neededInput);

                UpdateBuildingInfo?.Invoke(GetBuildingInfo());
                
                List<WorkerSnailBehavior> removedWorkers = new List<WorkerSnailBehavior>();
            
                foreach (var snail in workers)
                {
                    snail.gameObject.GetComponent<Snail>().NextTask();
                    if (snail.snail.IsMoving)
                    {
                        removedWorkers.Add(snail);
                    }
                }

                foreach (var snail in removedWorkers)
                {
                    workers.Remove(snail);
                }
            }
        }

        protected void EmptyOutput()
        {
            foreach (var snail in workers)
            {
                if (snail.snail.SatisfyingNeeds || snail.GetComponent<Snail>().inventorySlot.RemainingSpace <= 0) continue;
                
                Storage storage = BuildingRegistry.Instance.GetNearestStorageOfType(outputResource, InternalStreetPosition);
                
                if (storage == null) return;
                
                int amount = inventory.RemoveOutput(inventory.GetOutputAmount());
                
                snail.snail.inventorySlot.resourceType = outputResource;
                snail.snail.inventorySlot.currentAmount = Math.MinI(snail.snail.inventorySlot.maxAmount, amount);
                snail.SetTargetStorage(storage);
                workers.Remove(snail);
                        
                UpdateBuildingInfo?.Invoke(GetBuildingInfo());
                break;
            }
        }

        protected void FillInput()
        {
            if (sendSnailGetResources) return;
            print("empty input");
                
            Inventory targetInventory = BuildingRegistry.Instance.GetNearestOutputInventoryOfType(inputResource, InternalStreetPosition);

            if (targetInventory == null) return;
                
            foreach (var snail in workers)
            {
                if (snail.snail.SatisfyingNeeds || snail.snail.inventorySlot.RemainingSpace <= 0) continue;
                
                sendSnailGetResources = true;
                snail.GetResource(inputResource, targetInventory);
                workers.Remove(snail);
                        
                UpdateBuildingInfo?.Invoke(GetBuildingInfo());
                break;
            }
        }

        protected virtual bool CanProduce()
        {
            if (workers.Count <= 0) return false;
            if (inventory.GetInputAmount(inputResource) < neededInput) return false;
            if (inventory.GetOutputSpace() <= 0) return false;
            
            return true;
        }
        
        public void AddResources(int amount)
        {
            inventory.AddInput(inputResource, amount);
            sendSnailGetResources = false;
            UpdateBuildingInfo?.Invoke(GetBuildingInfo());
        }

        public override void SnailReached(SnailBehavior snail)
        {
            base.SnailReached(snail);
            print("[ProductionBuilding] SnailReached");
            if (workers.Contains(snail.gameObject.GetComponent<WorkerSnailBehavior>())) return;
            
            if(Vector3.Distance(snail.transform.position, GetSnailTargetPosition()) > 0.5)
            {
                snail.snail.SetDestination(GetSnailTargetPosition());
                return;
            }
            
            workers.Add(snail.gameObject.GetComponent<WorkerSnailBehavior>());
        }

        public override void DestroyBuilding()
        {
            base.DestroyBuilding();
            // for (int i = 0; i < requestedWorkerCount; i++)
            // {
            //     DecreaseWorkerCount();
            // }
            //
            // SnailBehavior[] tSnails = new SnailBehavior[snails.Count];
            // snails.CopyTo(tSnails); 
            // foreach (var snail in tSnails)
            // {
            //     snail.shouldBeUnAssigned = true;
            //     SnailReached(snail);
            // }
        }

        public override BuildingInfo GetBuildingInfo()
        {
            if (constructionSite != null) return constructionSite.GetConstructionInfo(BuildingData);
            
            Snail[] snails = new Snail[this.snails.Count];
            for (int i = 0; i < this.snails.Count; i++)
            {
                snails[i] = this.snails[i].snail;
            }

            InventorySlot inputSlot = new InventorySlot(ResourceType.None, 0);
            if (inventory.GetInputSlots().Length > 0)
            {
                inputSlot = inventory.GetInputSlots()[0];
            }
            return new BuildingInfo(this, BuildingData, snails, maxWorkerCount, inventory.GetOutputSlot(),inputSlot, productionTime);
        }
    }
}
