using System;
using Buildings;
using Resources;
using UnityEngine;
using UnityEngine.EventSystems;

namespace UI
{
    public class Tooltip : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] private string _title;
        public string Title => _title;
        
        [SerializeField] private string _description;
        public string Description => _description;

        [SerializeField] private BuildingData _buildingData;
        public BuildingData BuildingData => _buildingData;
        
        private TooltipObject tooltipObject;

        private void Start()
        {
            tooltipObject = FindObjectOfType<TooltipObject>();
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            tooltipObject.Show(this);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            tooltipObject.Hide(this);
        }
    }
}
