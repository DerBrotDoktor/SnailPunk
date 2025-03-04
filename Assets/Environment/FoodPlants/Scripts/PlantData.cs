using Resources;
using UnityEngine;

namespace Plants
{

    [CreateAssetMenu(fileName = "PlantData", menuName = "PlantData")]
    public class PlantData : ScriptableObject
    {
        [SerializeField] private ResourceType _resourceType;
        public ResourceType ResourceType => _resourceType;
        
        [SerializeField] private int _amount;
        public int Amount => _amount;
        
        [SerializeField] private float _timeToGrow;
        public float TimeToGrow => _timeToGrow;

        [SerializeField] private float _timeToHarvest;
        public float TimeToHarvest => _timeToHarvest;
        
        [SerializeField] private GameObject _prefab;
        public GameObject Prefab => _prefab;
        
        [SerializeField] private GameObject[] _models = new GameObject[4];
        public GameObject[] Models => _models;
    }
}
