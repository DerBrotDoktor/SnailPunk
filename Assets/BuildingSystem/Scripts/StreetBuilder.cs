using System;
using Sound;
using UI;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace BuildingSystem
{
    public class StreetBuilder : MonoBehaviour,IPointerDownHandler
    {

        [SerializeField] private MouseIndicator mouseIndicator;
        private StreetHandler streetHandler;

        private bool isBuilding = false;

        [SerializeField] private Color vignetteColor;
        [SerializeField] private string buildingTipText;
        
        [Header("Input")]
        [SerializeField] private InputActionAsset mouseAction;
        [SerializeField] private string actionMapName = "Mouse";
        [SerializeField] private string leftClickAction = "LeftMouseButton";
        private InputAction leftClick;
        private bool leftClickPressed = false;

        private Vector3 mousePosition => mouseIndicator.GetMousePosition;

        [Header("Sound")]
        [SerializeField] private SoundEmitter soundEmitter;

        private void Awake()
        {
            leftClick = mouseAction.FindActionMap(actionMapName).FindAction(leftClickAction);
            leftClick.Enable();
            // leftClick.started += context => { leftClickPressed = true; };
            // leftClick.canceled += context => { leftClickPressed = false; };
            
            streetHandler = FindObjectOfType<StreetHandler>();
            
            if (streetHandler == null)
            {
                throw new ArgumentException("No StreetHandler found in the scene.");
            }
        }

        private void OnEnable()
        {
            BuildBar.StreetButtonClicked += SetBuilding;
        }

        private void OnDisable()
        {
            BuildBar.StreetButtonClicked -= SetBuilding;
        }

        private void SetBuilding(bool value)
        {
            isBuilding = value;
            
            if (isBuilding)
            {
                FindObjectOfType<BuildModeInfo>().SetBuildModeInfo(BuildMode.Street);
            }
            else
            {
                FindObjectOfType<BuildModeInfo>().ClearBuildingModeInfo(BuildMode.Street);
            }
        }

        private void Update()
        {
            if (!isBuilding) return;
            
            if (Input.GetMouseButtonDown(1))
            {
                isBuilding = false;
                FindObjectOfType<BuildModeInfo>().ClearBuildingModeInfo(BuildMode.Street);
                return;
            }

            if (Input.GetMouseButtonUp(0) && leftClickPressed)
            {
                leftClickPressed = false;
            } 
            
            if (leftClickPressed)
            {
                if (!mouseIndicator.IsColliding && !streetHandler.HasTile(mousePosition))
                {
                    streetHandler.PlaceStreet(mousePosition);
                    soundEmitter.Play();
                }
            }
        }

        public Vector3 TryPlaceStreet(Vector3 position)
        {

            if (!streetHandler.HasTile(position))
            {
                streetHandler.PlaceStreet(position);
            }

            Vector3Int istreetPosition = streetHandler.GetTilePosition(position);
            Vector3 streetPosition = streetHandler.GetWorldPosition(istreetPosition);

            return streetPosition + new Vector3(0.5f,1f,0.5f);
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (Input.GetMouseButton(0))
            {
                leftClickPressed = true;
            }
        }

        public void DestroyAtPosition(Vector3 position)
        {
            streetHandler.DestroyStreet(position);
        }

        public void DestroyAtMouse()
        {
            DestroyAtPosition(mouseIndicator.GetCellPosition());
        }
    }
}
