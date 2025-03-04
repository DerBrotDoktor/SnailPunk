using System;
using Plants;
using Sound;
using UI;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Tilemaps;

namespace BuildingSystem
{
    public class FieldBuilder : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        [SerializeField] private GameObject fieldPrefab;
        [SerializeField] private Tilemap tileMap;
        [SerializeField] private MouseIndicator mouseIndicator;

        private PlantData currentData;

        private bool isBuilding = false;
        private bool leftClickPressed = false;

        [Header("Sound")]
        [SerializeField] private SoundEmitter soundEmitter;
        
        private void OnEnable()
        {
            BuildBar.FieldButtonClicked += SetBuilding;
        }

        private void OnDisable()
        {
            BuildBar.FieldButtonClicked -= SetBuilding;
        }

        private void SetBuilding(PlantData data)
        {
            if (data == null)
            {
                isBuilding = false;
                FindObjectOfType<BuildModeInfo>().ClearBuildingModeInfo(BuildMode.Field);
            }
            else
            {
                currentData = data;
                isBuilding = true;
                FindObjectOfType<BuildModeInfo>().SetBuildModeInfo(BuildMode.Field);
            }
        }

        private void Update()
        {
            if (!isBuilding) return;
            
            if (isDrawing)
            {
                UpdateDrawing();
            }
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (Input.GetMouseButtonDown(1))
            {
                isBuilding = false;
                StopDrawing();
                FindObjectOfType<BuildModeInfo>().ClearBuildingModeInfo(BuildMode.Field);
                return;
            }
            
            if (isBuilding && Input.GetMouseButton(0))
            {
                StartDrawing();
            }
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (isDrawing && !Input.GetMouseButton(0))
            {
                StopDrawing();
            }
        }

        private bool isDrawing = false;
        private Vector3 startPosition;
        private Vector3 endPosition;
        private GameObject currentSquare;
        [SerializeField] private GameObject squarePrefab;
        [SerializeField] private LayerMask layerMask;

        private void StartDrawing()
        {
            isDrawing = true;
            startPosition = mouseIndicator.transform.position;
            currentSquare = Instantiate(squarePrefab, (startPosition) + tileMap.cellSize / 2, Quaternion.Euler(90, 0, 0));
        }

        private void UpdateDrawing()
        {
            if (isDrawing)
            {
                endPosition = mouseIndicator.transform.position;
                UpdateSquare();
            }
        }

        private void StopDrawing()
        {
            isDrawing = false;
            Destroy(currentSquare);

            if (!isBuilding) return;

            Vector3 posOne = new Vector3(Math.Min(startPosition.x, endPosition.x),mouseIndicator.GetMousePosition.y, Math.Min(startPosition.z, endPosition.z));
            Vector3 posTwo = new Vector3(Math.Max(startPosition.x, endPosition.x),mouseIndicator.GetMousePosition.y, Math.Max(startPosition.z, endPosition.z));
            
            float xDiff = Math.Abs(posOne.x - posTwo.x);
            float zDiff = Math.Abs(posOne.z - posTwo.z);
            
            for (float x = 0; x <= xDiff; x++)
            {
                for (float z = 0; z <= zDiff; z++)
                {
                    Vector3 pos = new Vector3(posOne.x + x, posOne.y, posOne.z +z);
                    Vector3 worldPos = tileMap.CellToWorld(tileMap.WorldToCell(pos)) + (new Vector3(tileMap.cellSize.x, 0, tileMap.cellSize.z) / 2);
                    worldPos.y = pos.y;
                    
                    if(Physics.OverlapBox(worldPos, new Vector3(0.3f, 0.3f, 0.3f), Quaternion.identity, layerMask).Length > 0) continue;
                    
                    Field field = Instantiate(fieldPrefab, worldPos, Quaternion.identity).GetComponent<Field>();
                    field.OnBuild(currentData);
                }
            }

            soundEmitter.Play();
            
        }

        private void UpdateSquare()
        {
            Vector3 center = (startPosition + endPosition) / 2;
            center.y = mouseIndicator.transform.position.y;
            Vector3 size = new Vector3(Mathf.Abs(startPosition.x - endPosition.x), Mathf.Abs(startPosition.z - endPosition.z), 1);
            currentSquare.transform.position = center;
            currentSquare.transform.localScale = size + new Vector3(1,1,0);
        }
    }
}
