using System;
using System.Collections;
using Buildings;
using Resources;
using UnityEngine;
using Random = UnityEngine.Random;
using Vector3 = UnityEngine.Vector3;

namespace Snails
{
    [RequireComponent(typeof(BoxCollider), typeof(Rigidbody))]
    public class SnailNeeds : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer overheadIcon;
        
        private const int NEED_AMOUNT = 3;
        [SerializeField] private SnailNeed[] needs = new SnailNeed[NEED_AMOUNT];/// <summary> prioritised foods </summary>
        
        [SerializeField] private ResourceType[] prioritisedFoods = new ResourceType[5];

        private bool shouldTick = true;
        public House House{get; private set;}

        public SnailNeedType CurrentType { get; private set; } = SnailNeedType.None;
        private Inventory currentTargetInventory;
        
        public bool IsSatisfying{get; private set;}
        
        public Action FinishedNeedsSatisfying;

        private Snail snail;

        private bool goSleep = false;

        [SerializeField,Range(0, 100)] private float reproductionProbability = 1f;
        
        public bool IsDead{get; private set;}
        
        private void Awake()
        {
            snail = GetComponent<Snail>();
        }

        private void Start()
        {
            StartCoroutine(Tick());
        }

        private void OnEnable()
        {
            DayNightCycle.NightStarted += OnNightStarted;
        }

        private void OnDisable()
        {
            DayNightCycle.NightStarted -= OnNightStarted;
        }

        private IEnumerator Tick()
        {
            while (shouldTick)
            {
                for (int i = 0; i < needs.Length; i++)
                {
                    if (needs[i].Amount <= 0)
                    {
                        Die(needs[i].DeathMessage);
                        break;
                    }
                    
                    needs[i].Tick();
                    switch (needs[i].Type)
                    {
                        case SnailNeedType.Sleep:
                        {
                            snail.InfoData.SleepPercent = (float)needs[i].Amount / needs[i].MaxAmount * 100f;
                            break;
                        }
                        case SnailNeedType.Food:
                        {
                            snail.InfoData.FoodPercent = (float)needs[i].Amount / needs[i].MaxAmount * 100f;
                            break;
                        }
                        case SnailNeedType.Water:
                        {
                            snail.InfoData.WaterPercent = (float)needs[i].Amount / needs[i].MaxAmount * 100f;
                            break;
                        }
                    }
                }
                
                snail.InfoData.NeedsChanged?.Invoke();
                
                CheckComplain();
                
                yield return new WaitForSeconds(1);            
            }
        }

        private void CheckComplain()
        {
            foreach (var need in needs)
            {
                if (need.CurrentlyComplaining)
                {
                    overheadIcon.sprite = need.Icon;
                    overheadIcon.gameObject.SetActive(true);
                    return;
                }
            }
            
            overheadIcon.gameObject.SetActive(false);
        }

        public bool ShouldSatisfyNeed(out SnailNeedType needType)
        {
            if (goSleep)
            {
                needType = SnailNeedType.Sleep;
                CurrentType = needType;
                return true;
            }
            
            foreach (SnailNeed need in needs)
            {
                if (need.Amount >= need.MaxAmount) continue;
                if (need.Amount >= need.SatisfyThreshold)
                {
                    int rand = Random.Range(1, 100);
                    if (rand > need.SatisfyBeforeThresholdProbability)
                    {
                        continue;
                    }
                }
                
                needType = need.Type;
                CurrentType = needType;
                return true;
            }

            CurrentType = SnailNeedType.None;
            needType = SnailNeedType.None;
            return false;
        }

        public void SatisfyNeed()
        {
            IsSatisfying = true;
            
            switch (CurrentType)
            {
                case SnailNeedType.Sleep:
                {
                    if(goSleep) goSleep = false;
                    Reproduce();
                    StartCoroutine(SatisfySleep());
                    break;
                }
                case SnailNeedType.Food:
                {
                    int i = Array.FindIndex(needs, need => need.Type == SnailNeedType.Food);
                    int amount = Utils.Math.MinI(currentTargetInventory.GetOutputAmount() * needs[i].SatisfyAmount, needs[i].MaxAmount - needs[i].Amount);
                    
                    if(amount < 0) amount = 0;
                    
                    needs[i].Amount += amount;
                    currentTargetInventory.RemoveOutput(amount/needs[i].SatisfyAmount);
                    
                    CurrentType = SnailNeedType.None;
                    currentTargetInventory = null;
                    IsSatisfying = false;
                    snail.NextTask();
                    
                    break;
                }
                case SnailNeedType.Water:
                {
                    int i = Array.FindIndex(needs, need => need.Type == SnailNeedType.Water);
                    int amount = Utils.Math.MinI(currentTargetInventory.GetOutputAmount() * needs[i].SatisfyAmount, needs[i].MaxAmount - needs[i].Amount);
                    print("NEED: Amount: " + amount);
                    
                    if(amount < 0) amount = 0;
                    
                    needs[i].Amount += amount;
                    currentTargetInventory.RemoveOutput(amount/needs[i].SatisfyAmount);
                    
                    CurrentType = SnailNeedType.None;
                    currentTargetInventory = null;
                    IsSatisfying = false;
                    snail.NextTask();
                    
                    break;
                }
            }
            CheckComplain();
        }
        
        private IEnumerator SatisfySleep()
        {
            int i = Array.FindIndex(needs, need => need.Type == SnailNeedType.Sleep);
            
            if (i == -1) throw new ArgumentException("SnailNeedType.Sleep not found in needs array " + this);

            needs[i].CurrentlySatisfying = true;
            while (needs[i].Amount < needs[i].MaxAmount && needs[i].CurrentlySatisfying)
            {
                if (CurrentType == SnailNeedType.Sleep && House == null)
                {
                    needs[i].CurrentlySatisfying = false;
                }
                else
                {
                    yield return new WaitForSeconds(1);
                }
            }

            needs[i].CurrentlySatisfying = false;
            CurrentType = SnailNeedType.None;
            IsSatisfying = false;
            snail.NextTask();
            CheckComplain();
        }

        public void OnHouseDemolish()
        {
            House.ADemolish -= OnHouseDemolish;
            House = null;
        }
        
        public Vector3 GetSleepPosition()
        {
            return House.InternalStreetPosition;
        }
        
        public Vector3? GetFoodPosition()
        {
            Inventory inventory = BuildingRegistry.Instance.GetNearestOutputInventoryOfType(ResourceType.Food, transform.position);

            if (inventory == null) return null;
            
            currentTargetInventory = inventory;
            return inventory.gameObject.GetComponent<Building>().GetSnailTargetPosition();
        }
        
        public Vector3? GetWaterPosition()
        {
            Inventory inventory = BuildingRegistry.Instance.GetNearestOutputInventoryOfType(ResourceType.Drink, transform.position);
            
            if (inventory == null) return null;
            
            currentTargetInventory = inventory;
            return inventory.gameObject.GetComponent<Building>().GetSnailTargetPosition();
        }
        
        public void SetHouse(House house)
        {
            this.House = house;
            snail.InfoData.House = house;
            snail.InfoData.DataChanged?.Invoke();
        }

        private void Die(string cause)
        {
            IsDead = true;
            shouldTick = false;
            snail.Die(cause);
        }
        
        private void OnNightStarted()
        {
            goSleep = true;
        }

        private void Reproduce()
        {
            
            float rand = Random.Range(0f, 100f);
            if (rand < reproductionProbability)
            {
                if (GlobalInventory.Instance.GetResource(ResourceType.HouseSpace) <= GlobalInventory.Instance.GetResource(ResourceType.Snail)) return;
                FindObjectOfType<TownHall>().SpawnSnail(transform.position);
            }
        }
    }
}
