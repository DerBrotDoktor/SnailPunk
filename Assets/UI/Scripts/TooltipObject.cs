using System;
using Buildings;
using Resources;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

namespace UI
{
    public class TooltipObject : MonoBehaviour
    {
        [SerializeField] private Vector3 offset = new Vector3(0, 50, 0);
        [SerializeField] private TMP_Text text;
        [SerializeField] private GameObject container;
        private Tooltip currentTooltip;
        
        [SerializeField] private ToolTipBuildingCost buildingCostTooltipPrefab;
        [SerializeField] private GameObject buildingCostTooltipContainer;
        [SerializeField] private ItemIconDirectory itemIconDirectory;

        private void Awake()
        {
            container.SetActive(false);
        }

        public void Show(Tooltip tooltip)
        {
            currentTooltip = tooltip;
            transform.position = currentTooltip.transform.position + offset;
            
            text.text = "<b>" + currentTooltip.Title + "</b>";
            text.text += "\n" + currentTooltip.Description;

            foreach (Transform trans in buildingCostTooltipContainer.transform)
            {
                Destroy(trans.gameObject);
            }

            if (currentTooltip.BuildingData != null)
            {
                foreach (var cost in currentTooltip.BuildingData.BuildingCosts)
                {
                    if(cost.resourceType == ResourceType.None || cost.amount <= 0) continue;
                    
                    var buildingCostTooltip = Instantiate(buildingCostTooltipPrefab, buildingCostTooltipContainer.transform);
                    buildingCostTooltip.Initialize(itemIconDirectory.GetSprite(cost.resourceType), cost.amount.ToString());
                }
            }
            
            container.SetActive(true);
        }
        
        public void Hide(Tooltip tooltip)
        {
            if (currentTooltip != tooltip) return;
            
            currentTooltip = null;
            container.SetActive(false);
        }
    }
}