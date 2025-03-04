using System;
using Buildings;
using Plants;
using Sound;
using UI;
using UnityEngine;

namespace BuildingSystem
{
    public class ObjectDestroyer : MonoBehaviour
    {
        public static bool IsDestroying = false;
        
        [SerializeField] private StreetBuilder streetBuilder;
        
        [SerializeField] private LayerMask layerMask;

        [Header("Sound")]
        [SerializeField] private SoundEmitter soundEmitter;

        private void OnEnable()
        {
            BuildBar.DestroyBuildingButtonClicked += OnDestroyBuildingButtonClicked;
        }

        private void OnDisable()
        {
            BuildBar.DestroyBuildingButtonClicked -= OnDestroyBuildingButtonClicked;
        }
        
        private void Update()
        {
            if(!IsDestroying) return;

            if (Input.GetMouseButtonDown(0))
            {
                ExecuteDestroy();
            }

            if (Input.GetMouseButtonDown(1))
            {
                StopDestroy();
                FindObjectOfType<BuildModeInfo>().ClearBuildingModeInfo(BuildMode.Destroy);
            }
        }

        private void OnDestroyBuildingButtonClicked(bool value)
        {
            if (value)
            {
                StartDestroy();
                FindObjectOfType<BuildModeInfo>().SetBuildModeInfo(BuildMode.Destroy);
            }
            else
            {
                StopDestroy();
                FindObjectOfType<BuildModeInfo>().ClearBuildingModeInfo(BuildMode.Destroy);
            }
        }
        
        private void ExecuteDestroy()
        {
            RaycastHit hit;
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit,Mathf.Infinity, layerMask))
            {
                print("Destroy: " + hit.collider.gameObject.name);
                if(hit.collider.gameObject.TryGetComponent<Building>(out var building))
                {
                    building.DestroyBuilding(); 
                    soundEmitter.Play();
                    return;
                }

                if(hit.collider.gameObject.TryGetComponent<Field>(out var field))
                {
                    field.Demolish();
                    soundEmitter.Play();
                    return;
                }

                if (hit.collider.gameObject.CompareTag("Street"))
                {
                    streetBuilder.DestroyAtMouse();
                    soundEmitter.Play();
                    return;
                }
            }
        }
        
        private void StartDestroy()
        {
            IsDestroying = true;
            print("Destroy: started");
        }
        
        private void StopDestroy()
        {
            IsDestroying = false;
            print("Destroy: stopped");
        }
    }
}
