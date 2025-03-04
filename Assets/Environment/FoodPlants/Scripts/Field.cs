using BuildingSystem;
using Resources;
using UnityEngine;

namespace Plants
{
    public class Field : MonoBehaviour
    {

        public float timeToPlant = 5f;
        public bool IsSnailTarget = false;

        [SerializeField] private int fieldLayer;
        [SerializeField] private int plantLayer;
        private GameObject plantPrefab;
        private Plant plant;

        public ResourceType ResourceType => plant.ResourceType;

        public void OnBuild(PlantData plantData)
        {
            plantPrefab = plantData.Prefab;
            plant = Instantiate(plantPrefab, transform.position, Quaternion.identity, transform).GetComponent<Plant>();
        }

        public void Plant()
        {
            IsSnailTarget = false;
            plant.OnPlanted();
        }
        
        public bool CanBePlanted()
        {
            return !plant.IsPlanted();
        }

        public void Demolish()
        {
            MouseIndicator mouseIndicator = FindObjectOfType<MouseIndicator>();
            
            Collider coll = GetComponent<Collider>();
            coll.enabled = false;
            mouseIndicator.OnTriggerDestroy(coll);

            foreach (Transform child in transform)
            {
                Collider childColl = child.GetComponent<Collider>();
                if(childColl != null)
                {
                    childColl.enabled = false;
                    mouseIndicator.OnTriggerDestroy(childColl);
                }
            }
            
            Destroy(gameObject);
        }
    }
}
