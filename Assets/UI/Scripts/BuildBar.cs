using System;
using System.Collections.Generic;
using Buildings;
using Plants;
using TMPro;
using UnityEngine;

namespace UI
{
    public class BuildBar : MonoBehaviour
    {
        [SerializeField] private TMP_Text rotateTip;
        
        [SerializeField] private List<GameObject> panels;
        
        public static Action<BuildingData> BuildingButtonClicked;
        public static Action<PlantData> FieldButtonClicked;
        public static Action<bool> StreetButtonClicked;
        public static Action<bool> DestroyBuildingButtonClicked;

        private void Start()
        {
            foreach (var panel in panels)
            {
                panel.SetActive(false);
            }
        }

        public void OnPanelHoverStopped(string panelName)
        {
            foreach (var panel in panels)
            {
                if(panel.gameObject.name == panelName)
                {
                    panel.gameObject.SetActive(false);
                    return;
                }
            }
        }
        
        public void OnPanelHoverStarted(string panelName)
        {
            foreach (var panel in panels)
            {
                panel.gameObject.SetActive(panel.gameObject.name == panelName);
            }
        }

        public void OnBuildingButtonClicked(BuildingData buildingData)
        {
            BuildingButtonClicked?.Invoke(buildingData);
            FieldButtonClicked?.Invoke(null);
            StreetButtonClicked?.Invoke(false);
            DestroyBuildingButtonClicked?.Invoke(false);
        }

        public void OnFieldButtonClicked(PlantData plantData)
        {
            BuildingButtonClicked?.Invoke(null);
            FieldButtonClicked?.Invoke(plantData);
            StreetButtonClicked?.Invoke(false);
            DestroyBuildingButtonClicked?.Invoke(false);
        }
        
        public void OnStreetButtonClicked()
        {
            BuildingButtonClicked?.Invoke(null);
            FieldButtonClicked?.Invoke(null);
            StreetButtonClicked?.Invoke(true);
            DestroyBuildingButtonClicked?.Invoke(false);
        }
        
        public void OnDestroyBuildingButtonClicked()
        {
            BuildingButtonClicked?.Invoke(null);
            FieldButtonClicked?.Invoke(null);
            StreetButtonClicked?.Invoke(false);
            DestroyBuildingButtonClicked?.Invoke(true);
        }

        
    }
}
