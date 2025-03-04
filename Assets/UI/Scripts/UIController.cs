using System;
using Buildings;
using Snails;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class UIController : MonoBehaviour
    {
        public static Action<Building> BuildingClicked;
        public static Action<Snail> SnailClicked;
        
        [SerializeField] private GameObject buildingMenu;
        [SerializeField] private BuildingInfoPanel buildingInfoPanel;
        [SerializeField] private SnailInfoPanel snailInfoPanel;

        private void OnEnable()
        {
            BuildingClicked += OnBuildingClicked;
            SnailClicked += OnSnailClicked;
        }
        
        private void OnDisable()
        {
            BuildingClicked -= OnBuildingClicked;
            SnailClicked -= OnSnailClicked;
        }

        private void OnBuildingClicked(Building building)
        {
            snailInfoPanel.ClosePanel();
            buildingInfoPanel.SetBuilding(building);
            buildingInfoPanel.OpenPanel();
        }
        
        private void OnSnailClicked(Snail snail)
        {
            buildingInfoPanel.ClosePanel();
            snailInfoPanel.SetSnail(snail);
            snailInfoPanel.OpenPanel();
        }

        private void OnSetActiveBuildingMenu(bool value)
        {
            buildingMenu.SetActive(value);
            
            if (!value) return;

            buildingInfoPanel.ClosePanel();
            snailInfoPanel.ClosePanel();
        }
    }
}
