using System;
using Buildings;
using FMOD;
using Resources;
using UI;
using Unity.Mathematics;
using UnityEngine;

namespace Snails
{
    [RequireComponent(typeof(Movement),typeof(SnailNeeds))]
    public class Snail : MonoBehaviour
    {
        private SnailType type;
        
        [Header("Models")]
        [SerializeField] private GameObject deadModelPrefab;
        [SerializeField] private GameObject noneModelPrefab;
        [SerializeField] private GameObject harvestingModelPrefab;
        [SerializeField] private GameObject workerModelPrefab;
        [SerializeField] private GameObject constructionModelPrefab;
        [SerializeField] private GameObject farmModelPrefab;
        
        public Building AssignedBuilding { get; private set; }

        private SnailBehavior currentBehavior;
        private GameObject currentModel;
        
        private Movement movement;
        private SnailNeeds needs;
        
        public bool ShouldSatisfyNeeds => targetNeedType != SnailNeedType.None;
        public bool SatisfyingNeeds => needs.IsSatisfying && targetNeedType != SnailNeedType.None;
        private SnailNeedType targetNeedType = SnailNeedType.None;
        public House House => needs.House;

        public InventorySlot inventorySlot;

        public SnailInfoData InfoData;
        
        public bool IsDead => needs.IsDead;
        public bool IsMoving => movement.IsMoving;
        
        private void Awake()
        {
            movement = GetComponent<Movement>();
            movement.TargetReached += OnTargetReached;
            
            needs = GetComponent<SnailNeeds>();
            needs.FinishedNeedsSatisfying += NextTask;
            ChangeType(SnailType.None);

            InfoData = new SnailInfoData();
        }

        public void Initialize(string name)
        {
            InfoData.Name = name;
            Initialize();
        }
        
        public void Initialize()
        {
            if (BuildingRegistry.Instance.TryGetHouse(out var house))
            {
                AssignHouse(house);
            }
            else
            {
                BuildingRegistry.Instance.NewHouseBuild += OnNewHouseBuild;
            }
            
            DayNightCycle.NewDay += OnNewDay;
        }
        
        public void ChangeType(SnailType type)
        {
            print("[Snail] Change type: " + type);
            this.type = type;
            
            if (currentBehavior != null)
            {
                Destroy(currentBehavior);
            }

            if (currentModel != null)
            {
                Destroy(currentModel.gameObject);
            }

            switch (type)
            {
                case SnailType.None:
                    currentModel = Instantiate(noneModelPrefab, transform);
                    currentBehavior = gameObject.AddComponent<SnailBehavior>();
                    break;
                case SnailType.Harvester:
                    currentModel = Instantiate(harvestingModelPrefab, transform);
                    currentBehavior = gameObject.AddComponent<HarvestingSnailBehavior>();
                    break;
                case SnailType.Worker:
                    currentModel = Instantiate(workerModelPrefab, transform);
                    currentBehavior = gameObject.AddComponent<WorkerSnailBehavior>();
                    break;
                case SnailType.Planter:
                    currentModel = Instantiate(workerModelPrefab, transform);
                    currentBehavior = gameObject.AddComponent<PlantSnailBehavior>();
                    break;
                case SnailType.Builder:
                    currentModel = Instantiate(constructionModelPrefab, transform);
                    currentBehavior = gameObject.AddComponent<ConstructionSnailBehavior>();
                    break;
                case SnailType.Farmer:
                    currentModel = Instantiate(farmModelPrefab, transform);
                    currentBehavior = gameObject.AddComponent<FarmSnailBehavior>();
                    break;
                case SnailType.Military:
                default:
                    throw new Exception("[Snail] Not supported type: " + type);
                    break;

            }
        }

        private void OnNewHouseBuild(House house)
        {
            BuildingRegistry.Instance.NewHouseBuild -= OnNewHouseBuild;
            AssignHouse(house);
        }

        private void AssignHouse(House house)
        {
            if(!house.TryAddSnail(this))
            { 
                BuildingRegistry.Instance.NewHouseBuild += OnNewHouseBuild;
                return;
            }
            needs.SetHouse(house);
            house.ADemolish += OnHouseDemolish;

            if (needs.IsSatisfying && needs.CurrentType == SnailNeedType.Sleep)
            {
                SetDestination(house.InternalStreetPosition);
            }
        }
        
        public void SetDestination(Vector3 position, bool force = false)
        {
            movement.SetDestination(position, force);
        }

        public void NextTask()
        {
            
            if (targetNeedType != SnailNeedType.None && !needs.IsSatisfying)
            {
                targetNeedType = SnailNeedType.None;
                needs.SatisfyNeed();
                return;
            }
            
            if (needs.IsSatisfying) return;
            
            if (!CheckNeeds())
            {
                currentBehavior.NextTask();
            }
        }

        public bool CheckNeeds()
        {
            if (!needs.ShouldSatisfyNeed(out SnailNeedType needType)) return false;
            print("[Snail] NEED: " + needType);
            switch (needType)
            {
                case SnailNeedType.Sleep when needs.House != null:
                {
                    SetDestination(needs.GetSleepPosition());
                    targetNeedType = SnailNeedType.Sleep;
                    return true;
                }
                case SnailNeedType.Food:
                {
                    var foodPosition = needs.GetFoodPosition();
                    
                    if (foodPosition == null) return false;
                    
                    SetDestination(foodPosition.Value);
                    targetNeedType = SnailNeedType.Food;

                    return true;
                }
                case SnailNeedType.Water:
                {
                    var waterPosition = needs.GetWaterPosition();
                    
                    if (waterPosition == null) return false;
                    
                    SetDestination(waterPosition.Value);
                    targetNeedType = SnailNeedType.Water;
                    
                    return true;
                }
                case SnailNeedType.None:
                default:
                    return false;
            }
        }

        private void OnTargetReached()
        {
            currentBehavior.OnTargetReached();
        }

        public void AssignBuilding(Building building)
        {
            if (building == null)
            {
                AssignedBuilding = null;
                ChangeType(SnailType.None);
                return;
            }
            print("[Snail] Assigning building to snail: " + this + " " + building);
            ChangeType(building.WorkerType);
            
            AssignedBuilding = building;
            currentBehavior.SetAssignedBuilding(AssignedBuilding);
            
            SetDestination(building.InternalStreetPosition);
            
            InfoData.AssignedBuilding = AssignedBuilding;
            InfoData.DataChanged?.Invoke();
        }

        private void OnHouseDemolish()
        {
            needs.OnHouseDemolish();
            BuildingRegistry.Instance.NewHouseBuild += OnNewHouseBuild;
        }

        private void OnNewDay()
        {
            InfoData.Age++;
            InfoData.DataChanged?.Invoke();
        }

        public void Die(string cause)
        {
            FindObjectOfType<MessageInfoPanel>().NewMessage(InfoData.Name + " " + cause + " at age " + InfoData.Age);

            GlobalInventory.Instance.RemoveResource(ResourceType.Snail, 1);
            
            if(needs.House != null) needs.House.RemoveSnail(this);
            
            movement.SetDestination(transform.position, true);
            currentBehavior.SetAssignedBuilding(null);
            currentBehavior.enabled = false;

            Instantiate(deadModelPrefab, transform.position, quaternion.identity, null);
            currentModel.SetActive(false);

            Destroy(gameObject);
        }
    }
}