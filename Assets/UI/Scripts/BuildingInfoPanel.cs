using Buildings;
using Resources;
using Sound;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace UI
{
    public class BuildingInfoPanel : MonoBehaviour
    {
        [SerializeField] private GameObject container;
        [FormerlySerializedAs("iconDirectory")] [SerializeField] private ItemIconDirectory itemIconDirectory;

        [Header("Sound")]
        [SerializeField] private BuildingInfoPanelSoundEmitter openSoundEmitter;
        [SerializeField] private SoundEmitter closeSoundEmitter;
        
        [Header("Name")]
        [SerializeField] private TMP_Text nameText;
        [SerializeField] private Image icon;
        
        [Header("Description")]
        [SerializeField] private TMP_Text descriptionText;
        
        [Header("Worker")]
        [SerializeField] private GameObject workerContainer;
        [SerializeField] private TMP_Text workerAmountText;
        [SerializeField] private BuildingInfoWorkerPanel[] workerPanel = new BuildingInfoWorkerPanel[4];
        
        [Header("Production")]
        [SerializeField] private GameObject productionContainer;
        [SerializeField] private Image productionInputIcon;
        [SerializeField] private GameObject productionInputContainer;
        [SerializeField] private Image productionOutputIcon;
        [SerializeField] private TMP_Text timeText;

        [Header("Inventory")]
        [SerializeField] private GameObject inventoryContainer;
        [SerializeField] private GameObject inputContainer;
        [SerializeField] private Image inputIcon;
        [SerializeField] private TMP_Text inputText;
        [SerializeField] private GameObject outputContainer;
        [SerializeField] private Image outputIcon;
        [SerializeField] private TMP_Text outputText;
        
        private BuildingInfo currentBuildingInfo;
        private Building currentBuilding;

        private void Start()
        {
            ClosePanel();
        }

        private void Update()
        {
            if (!container.activeSelf) return;
            
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                ClosePanel();
            }
        }

        public void OpenPanel()
        {
            container.SetActive(true);
            openSoundEmitter.Play(currentBuildingInfo.Name);
        }
        
        public void ClosePanel()
        {
            container.SetActive(false);
            closeSoundEmitter.Play();
        }

        public void SetBuilding(Building building)
        {
            if(currentBuilding != null) currentBuilding.UpdateBuildingInfo -= SetBuildingInfo;
            
            currentBuilding = building;
            currentBuilding.UpdateBuildingInfo += SetBuildingInfo;
            SetBuildingInfo(building.GetBuildingInfo());
        }
        
        private void SetBuildingInfo(BuildingInfo buildingInfo)
        {
            currentBuildingInfo = buildingInfo;
            
            nameText.text = buildingInfo.Name;
            
            if (buildingInfo.Icon == null)
            {
                icon.enabled = false;
            }
            else
            {
                icon.sprite = buildingInfo.Icon;
                icon.enabled = true;
            }
            
            descriptionText.text = buildingInfo.Description;
            
            workerContainer.SetActive(buildingInfo.MaxWorkerCount > 0);
            
            workerAmountText.text = buildingInfo.WorkerCount + " / " + buildingInfo.MaxWorkerCount;
            
            for (int j = 0; j < workerPanel.Length; j++)
            {
                if (j < buildingInfo.Snails.Length)
                {
                    workerPanel[j].SetSnail(buildingInfo.Snails[j]);
                    workerPanel[j].gameObject.SetActive(true);
                }
                else
                {
                    workerPanel[j].SetSnail(null);
                    workerPanel[j].gameObject.SetActive(false);
                }
            }
            
            if(buildingInfo.OutputSlot.resourceType == ResourceType.None)
            {
                inventoryContainer.SetActive(false);
            }
            else
            {
                inventoryContainer.SetActive(true);

                outputText.text = buildingInfo.OutputSlot.currentAmount + " / " + buildingInfo.OutputSlot.maxAmount;
                outputIcon.sprite = itemIconDirectory.GetSprite(buildingInfo.OutputSlot.resourceType);
                
                if (buildingInfo.InputSlot.resourceType != ResourceType.None)
                {
                    inputContainer.SetActive(true);
                    
                    inputText.text = buildingInfo.InputSlot.currentAmount + " / " + buildingInfo.InputSlot.maxAmount;
                    inputIcon.sprite = itemIconDirectory.GetSprite(buildingInfo.InputSlot.resourceType);
                }
                else
                {
                    inputContainer.SetActive(false);
                }
            }
            
            if(buildingInfo.ProductionTime < 0)
            {
                productionContainer.SetActive(false);
            }
            else
            {
                if (buildingInfo.InputSlot.resourceType == ResourceType.None)
                {
                    productionInputContainer.SetActive(false);
                }
                else
                {
                    productionInputContainer.SetActive(true);
                    productionInputIcon.sprite = itemIconDirectory.GetSprite(buildingInfo.InputSlot.resourceType);
                }
                
                productionContainer.SetActive(true);
                
                productionOutputIcon.sprite = itemIconDirectory.GetSprite(buildingInfo.OutputSlot.resourceType);
                
                timeText.text = buildingInfo.ProductionTime.ToString() + " s";
            }
        }

        public void OnViewButtonClicked()
        {
            FindObjectOfType<CameraController>().LookAtTarget(currentBuilding.transform);
        }

        public void OnCloseButtonClicked()
        {
            ClosePanel();
        }
        
        public void IncreaseWorkerCount()
        {
            currentBuilding.IncreaseWorkerCount();
        }
        
        public void DecreaseWorkerCount()
        {
            currentBuilding.DecreaseWorkerCount();
        }
    }
}
