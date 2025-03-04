using System;
using Snails;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class SnailInfoPanel : MonoBehaviour
    {
        [SerializeField] private GameObject container;
        
        [Header("Text")]
        [SerializeField] private TMP_Text nameText;
        [SerializeField] private TMP_Text ageText;
        [SerializeField] private TMP_Text houseText;
        [SerializeField] private TMP_Text buildingText;
        
        [Header("Backup Text")]
        [SerializeField] private String houseNameBackup;
        [SerializeField] private String buildingNameBackup;
        
        [Header("Sliders")]
        [SerializeField] private Slider waterSlider;
        [SerializeField] private Slider foodSlider;
        [SerializeField] private Slider sleepSlider;

        private PortraitCamera portraitCamera;
        
        private Snail currentSnail;

        private void Start()
        {
            portraitCamera = FindObjectOfType<PortraitCamera>();
            ClosePanel();
        }

        public void OpenPanel()
        {
            container.SetActive(true);
        }

        public void ClosePanel()
        {

            if (currentSnail != null)
            {
                currentSnail.InfoData.DataChanged -= OnDataChanged;
                currentSnail.InfoData.NeedsChanged -= OnNeedsChanged;
            }

            container.SetActive(false);
            currentSnail = null;
        }

        public void SetSnail(Snail snail)
        {
            ClosePanel();
            
            currentSnail = snail;
            portraitCamera.SetTarget(snail.gameObject);
            
            currentSnail.InfoData.DataChanged += OnDataChanged;
            currentSnail.InfoData.NeedsChanged += OnNeedsChanged;
            
            OnDataChanged();
            OnNeedsChanged();
        }

        private void OnDataChanged()
        {
            nameText.text = currentSnail.InfoData.Name;
            ageText.text = "Age: " + currentSnail.InfoData.Age.ToString();
            houseText.text = currentSnail.InfoData.House?.BuildingName ?? houseNameBackup;
            buildingText.text = currentSnail.InfoData.AssignedBuilding?.BuildingName ?? buildingNameBackup;
        }

        private void OnNeedsChanged()
        {
            waterSlider.value = currentSnail.InfoData.WaterPercent / 100f;
            foodSlider.value = currentSnail.InfoData.FoodPercent / 100f;
            sleepSlider.value = currentSnail.InfoData.SleepPercent / 100f;
        }

        public void OnCloseButtonClicked()
        {
            ClosePanel();
            portraitCamera.SetTarget(null);
        }

        public void OnViewButtonClicked()
        {
            FindObjectOfType<CameraController>().LookAtTarget(currentSnail.gameObject.transform);
        }

        public void OnHouseButtonClicked()
        {
            if (currentSnail.House == null) return;
            FindObjectOfType<CameraController>().LookAtTarget(currentSnail.House.transform);
        }
        
        public void OnWorkButtonClicked()
        {
            if (currentSnail.AssignedBuilding == null) return;
            FindObjectOfType<CameraController>().LookAtTarget(currentSnail.AssignedBuilding.transform);
        }
    }
}