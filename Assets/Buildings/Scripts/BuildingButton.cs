using BuildingSystem;
using UnityEngine;

namespace Buildings
{
    public class BuildingButton : MonoBehaviour
    {
        [SerializeField] private BuildingData buildingData;
        
        public void OnClick()
        {
            BuildingBuilder.Instance.SelectBuilding(buildingData);
        }
    }
}
