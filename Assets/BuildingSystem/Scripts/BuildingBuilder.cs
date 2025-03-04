using System;
using System.Collections.Generic;
using Buildings;
using Sound;
using UI;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.Tilemaps;

namespace BuildingSystem
{
    public class BuildingBuilder : MonoBehaviour, IPointerDownHandler
    {
        public static BuildingBuilder Instance { get; private set; }

        
        [SerializeField] private TownHall townHall;
        
        [SerializeField] private Tilemap tilemap;
        [SerializeField] private MouseIndicator mouseIndicator;
        [SerializeField] private StreetIndicator streetIndicator;
        [SerializeField] private StreetBuilder streetBuilder;

        private Building buildingPrefab;
        [SerializeField] private BuildingData currentBuildingData;
        private Building buildingHolographic;
        [SerializeField] private Material hologramMaterial;
        
        [Header("Sound")]
        [SerializeField] private SoundEmitter buildSoundEmitter;
        [SerializeField] private SoundEmitter rotationSoundEmitter;
        
        private bool isBuilding = false;
        private bool isPositionValid = false;
        
        private Vector3 currentStreetPosition;
        
        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(this);
            }
            else
            {
                Instance = this;
            }
            
        }

        private void OnEnable()
        {
            BuildBar.BuildingButtonClicked += OnBuildingButtonClicked;
        }

        private void OnDisable()
        {
            BuildBar.BuildingButtonClicked -= OnBuildingButtonClicked;
        }

        private void OnBuildingButtonClicked(BuildingData buildingData)
        {
            if (buildingData == null)
            {
                StopBuilding();
                return;
            }
            
            SelectBuilding(buildingData);
        }
        
        private void Update()
        {
            if(!isBuilding) return;

            if (Input.GetMouseButtonDown(1))
            {
                StopBuilding();
                return;
            }

            if (Input.GetKeyDown(KeyCode.R))
            {
                Rotate();
            }
            
            currentStreetPosition = mouseIndicator.GetCellPosition() + CalculateRotatedPosition(currentBuildingData.ExternalStreetPosition, buildingHolographic.transform.rotation.eulerAngles.y);
            currentStreetPosition += new Vector3(tilemap.cellSize.x/2,0,tilemap.cellSize.z/2);
            streetIndicator.SetPosition(currentStreetPosition);
            
            Vector3 gridPos = mouseIndicator.GetCellPosition();
            buildingHolographic.transform.position = gridPos + currentBuildingData.CenterPosition;
            CheckPosition();
        }
    
        private void StartBuilding()
        {
            mouseIndicator.ReloadCollisions();
            mouseIndicator.SetColorize(true);
            mouseIndicator.SetSize(currentBuildingData.BuildingSize);
            
            isBuilding = true;

            currentStreetPosition = mouseIndicator.GetCellPosition() + currentBuildingData.ExternalStreetPosition;
            
            streetIndicator.SetPosition(currentStreetPosition);
            streetIndicator.SetVisibility(true);
            
            buildingHolographic = Instantiate(buildingPrefab, mouseIndicator.GetCellPosition() + new Vector3(0, 1000, 0), Quaternion.identity);
            buildingHolographic.tag = "Hologram";
            buildingHolographic.gameObject.layer = LayerMask.NameToLayer("Hologram");
            buildingHolographic.OnHologram(hologramMaterial);
            
            FindObjectOfType<BuildModeInfo>().SetBuildModeInfo(BuildMode.Building);
        }
    
        private void StopBuilding()
        {
            mouseIndicator.SetColorize(false);
            mouseIndicator.SetSize(new Vector3Int(1,1,1));
            mouseIndicator.SetRotation(0f);
            
            streetIndicator.SetVisibility(false);
            
            isBuilding = false;
            
            if(buildingHolographic == null) return;
            
            Destroy(buildingHolographic.gameObject);
            buildingHolographic = null;
            
            FindObjectOfType<BuildModeInfo>().ClearBuildingModeInfo(BuildMode.Building);
            
        }

        private void Build(Vector3 pos)
        {
            float rotationY = buildingHolographic.transform.rotation.eulerAngles.y;
            Debug.Log("Current Y Rotation: " + rotationY);

            Vector3 internalStreetPosition = Vector3.zero;
            
            if (!currentBuildingData.BuildingPrefab.gameObject.TryGetComponent<DecorativeBuilding>(out var _))
            {
                Vector3 rotatedInternalStreetPosition = CalculateRotatedPosition(currentBuildingData.InternalStreetPosition, rotationY);
                Vector3 rotatedExternalStreetPosition = CalculateRotatedPosition(currentBuildingData.ExternalStreetPosition, rotationY);


                internalStreetPosition = streetBuilder.TryPlaceStreet(pos + rotatedInternalStreetPosition);
                internalStreetPosition.y = 0;
                streetBuilder.TryPlaceStreet(pos + rotatedExternalStreetPosition);

            }

            Building building = Instantiate(currentBuildingData.BuildingPrefab, pos + currentBuildingData.CenterPosition, buildingHolographic.transform.rotation);
            building.OnBuild(currentBuildingData.BuildingSize, internalStreetPosition, currentBuildingData);
            buildSoundEmitter.Play();
        }

        private Vector3 CalculateRotatedPosition(Vector3 originalPosition, float rotationY)
        {
            int xSize = currentBuildingData.BuildingSize.x - 1;
            int zSize = currentBuildingData.BuildingSize.z - 1 ;
            
            if (Mathf.Approximately(rotationY, 270))
            {
                return new Vector3(-originalPosition.z + xSize, 0, originalPosition.x);
            }
            else if (Mathf.Approximately(rotationY, 180f))
            {
                return new Vector3(xSize - originalPosition.x, 0, -originalPosition.z + zSize);
            }
            else if (Mathf.Approximately(rotationY, 90))
            {
                return new Vector3(originalPosition.z, 0, zSize - originalPosition.x);
            }
            return originalPosition;
        }


        public void SelectBuilding(BuildingData buildingData)
        {
            StopBuilding();
            currentBuildingData = buildingData;
            buildingPrefab = buildingData.BuildingPrefab;
            StartBuilding();
        }
    
        private void CheckPosition()
        {
            isPositionValid = CheckPositionValid();
            mouseIndicator.SetValidPosition(isPositionValid);
        }

        private bool CheckPositionValid()
        {
            return !mouseIndicator.IsColliding;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            print("[BuildingBuilder] OnPointerDown");
            CheckPosition();
            if (Input.GetMouseButton(0) && isBuilding && isPositionValid && streetIndicator.IsValid)
            {
                Vector3 gridPos = mouseIndicator.GetCellPosition();
                Build(gridPos);
            }
        }

        private void Rotate()
        {
            float currentYRotation = buildingHolographic.transform.rotation.eulerAngles.y;
            float newYRotation = currentYRotation - 90f;
            
            buildingHolographic.transform.rotation = Quaternion.Euler(0f, newYRotation, 0f);
            mouseIndicator.SetRotation(newYRotation);
            
            rotationSoundEmitter.Play();
        }
    }
}
