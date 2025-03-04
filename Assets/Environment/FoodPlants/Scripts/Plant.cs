using System;
using BuildingSystem;
using Resources;
using Tutorial;
using UnityEngine;
using UnityEngine.Serialization;

namespace Plants
{
    public class Plant : MonoBehaviour
    {
        [SerializeField] private PlantData plantData;
        public ResourceType ResourceType => plantData.ResourceType;
        
        [SerializeField] private bool handPlaced = false;

        private GameObject model;
        private int growState = -1;

        private float timeBetweenGrow = 0f;
        private float timeSinceGrow = 0f;

        private bool isTargeted;

        private void Awake()
        {
            timeBetweenGrow = plantData.TimeToGrow / plantData.Models.Length;
            
            if(handPlaced)
            {
                model = Instantiate(plantData.Models[plantData.Models.Length -1], transform);
                growState = plantData.Models.Length - 1;
            }
        }

        public void OnPlanted()
        {
            growState = 0;
            model = Instantiate(plantData.Models[growState], transform);

            if(TryGetComponent<TutorialEventEmitter>(out TutorialEventEmitter emitter))
            {
                emitter.Emit();
            }
        }

        private void Update()
        {
            if (growState < 0 ||growState >= plantData.Models.Length - 1) return;
            
            timeSinceGrow += Time.deltaTime * Game.TimeScale;
            if (timeSinceGrow > timeBetweenGrow)
            {
                timeSinceGrow = 0f;
                growState++;
                Destroy(model);
                model = Instantiate(plantData.Models[growState], transform);
            }
        }
        
        public float GetHarvestTime()
        {
            return plantData.TimeToHarvest;
        }

        public int Harvest()
        {
            Destroy(model);
            growState = -1;
            
            isTargeted = false;

            if (handPlaced)
            {
                FindObjectOfType<MouseIndicator>().OnTriggerDestroy(GetComponent<Collider>());
                Destroy(gameObject, Time.deltaTime);
            }
            
            return plantData.Amount;
        }

        public bool TrySetTarget()
        {
            if(isTargeted) return false;
            
            isTargeted = true;
            return true;
        }
        
        public bool IsTargeted() => isTargeted;
        public bool CanBeHarvested() => growState >= plantData.Models.Length - 1;
        
        public bool IsPlanted() => growState >= 0;
    }
}
