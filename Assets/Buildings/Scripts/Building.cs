using System;
using System.Collections.Generic;
using BuildingSystem;
using Snails;
using Tutorial;
using UnityEngine;

namespace Buildings
{
    [RequireComponent(typeof(BoxCollider))]
    public class Building : MonoBehaviour
    {

        [Header("Building Model")] 
        [SerializeField] private GameObject model;

        [SerializeField] private ConstructionSite constructionSitePrefab;
        protected ConstructionSite constructionSite;
        
        [Header("Building")]
        [SerializeField] private BuildingType _buildingType;
        public BuildingType BuildingType => _buildingType;
        
        protected BuildingStatus Status { get; private set; } = BuildingStatus.Hologram;

        [SerializeField] private SnailType _workerType;
        public SnailType WorkerType => _workerType;

        private BoxCollider boxCollider;
        
        protected BuildingRegistry BuildingRegistry;

        public TownHall townHall;
        public int requestedWorkerCount = 0;
        public int sendWorkerCount = 0;
        public int maxWorkerCount = 1;

        public bool BuildingStopped = false;
        protected bool shouldBeDestroyed = false;
        
        public String BuildingName => BuildingData.BuildingName;        

        public Vector3 InternalStreetPosition { get; private set; }
        //TODO: External street position
        public Vector3 ExternalStreetPosition => (InternalStreetPosition - BuildingData.InternalStreetPosition) + BuildingData.ExternalStreetPosition;
        protected BuildingData BuildingData;

        [SerializeField] protected List<SnailBehavior> snails = new List<SnailBehavior>();
        
        public Action<BuildingInfo> UpdateBuildingInfo;
        
        protected virtual void Awake()
        {
            boxCollider = GetComponent<BoxCollider>();
            model.SetActive(false);
        }

        public virtual void OnBuild(Vector3Int colliderSize, Vector3 internalStreetPosition, BuildingData buildingData, bool construction = true)
        {
            Status = BuildingStatus.NoPath;
            boxCollider.size = new Vector3(colliderSize.x, 0.3f, colliderSize.z);
            boxCollider.center = Vector3.zero;

            InternalStreetPosition = internalStreetPosition;
            BuildingData = buildingData;

            FindMainBuilding();
            
            if (!TryGetComponent<TownHall>(out var tStorage))
            {
                FindTownHall();
            }
            else
            {
                townHall = tStorage;
            }
            
            if(construction)
            {
                constructionSite = Instantiate(constructionSitePrefab, transform.position, transform.rotation);
                constructionSite.Initialize(buildingData.BuildingCosts, this, townHall);
            }
            else
            {
                OnConstructed();
            }

            if(TryGetComponent<TutorialEventEmitter>(out var tutorialEmitter))
            {
                tutorialEmitter.Emit();
            }
        }

        public virtual void OnConstructed()
        {
            model.SetActive(true);
            
            BuildingRegistry.AddBuilding(this, BuildingType);

            if (maxWorkerCount > 0)
            {
                IncreaseWorkerCount();
            }
            
        }
        
        public virtual void Update()
        {
            if (Status == BuildingStatus.Hologram)
            {
                return;
            }
        }

        public virtual void OnHologram(Material hologramMaterial)
        {
            ChangeMaterial(hologramMaterial, transform);
            model.SetActive(true);
        }
        
        private void FindTownHall()
        {
            TownHall[] halls =  FindObjectsOfType<TownHall>();
            
            TownHall nearestTownHall = null;
            float nearestDistance = float.MaxValue;
            
            foreach (var hall in halls)
            {
                float distance = Vector3.Distance(hall.transform.position, transform.position);
                if (distance < nearestDistance)
                {
                    nearestTownHall = hall;
                    nearestDistance = distance;
                }
            }

            this.townHall = nearestTownHall;
        }

        private void FindMainBuilding()
        {
            BuildingRegistry[] mainBuildings =  FindObjectsOfType<BuildingRegistry>();
            
            BuildingRegistry nearestBuildingRegistry = null;
            float nearestDistance = float.MaxValue;
            
            foreach (var building in mainBuildings)
            {
                float distance = Vector3.Distance(building.transform.position, transform.position);
                if (distance < nearestDistance)
                {
                    nearestBuildingRegistry = building;
                    nearestDistance = distance;
                }
            }

            BuildingRegistry = nearestBuildingRegistry;
        }
        
        public virtual void AddSnail(SnailBehavior snail)
        {
            snails.Add(snail);
        }

        public virtual void RemoveSnail(SnailBehavior snail)
        {
            snails.Remove(snail);
            townHall.GetMoreWorkers(this);
        }

        public Vector3 GetSnailTargetPosition()
        {
            return InternalStreetPosition;
        }
        
        public virtual void SnailReached(SnailBehavior snail)
        {
            if (snail.shouldBeUnAssigned)
            {
                snails.Remove(snail);
                townHall.UnAssignSnail(snail.gameObject.GetComponent<Snail>());

                if (snails.Count <= 0)
                {
                    Demolish();
                }
            }
        }
        
        public virtual BuildingInfo GetBuildingInfo()
        {
            if (constructionSite != null) return constructionSite.GetConstructionInfo(BuildingData);
            
            Snail[] snails = new Snail[this.snails.Count];
            for (int i = 0; i < this.snails.Count; i++)
            {
                snails[i] = this.snails[i].snail;
            }
            
            return new BuildingInfo(this, BuildingData, snails, maxWorkerCount);
        }
        
        public void IncreaseWorkerCount()
        {
            if(requestedWorkerCount >= maxWorkerCount) return;
            requestedWorkerCount++;
            townHall.GetMoreWorkers(this);
            UpdateBuildingInfo?.Invoke(GetBuildingInfo());
        }
        
        public void DecreaseWorkerCount()
        {
            if(requestedWorkerCount <= 0) return;
            requestedWorkerCount--;
            snails[requestedWorkerCount].shouldBeUnAssigned = true;
            UpdateBuildingInfo?.Invoke(GetBuildingInfo());
        }

        public virtual void DestroyBuilding()
        {
            maxWorkerCount = 0;
            requestedWorkerCount = 0;
            
            foreach (var snailBehavior in snails)
            {
                snailBehavior.ForceUnAssign();
            }
            
            Demolish();
        }
        
        protected void Demolish()
        {
            BuildingRegistry.Instance.RemoveBuilding(this, BuildingType);

            if(constructionSite != null) constructionSite.Demolish();
            
            FindObjectOfType<StreetBuilder>().DestroyAtPosition(InternalStreetPosition - new Vector3(0, 1, 0));
            boxCollider.enabled = false;
            FindObjectOfType<MouseIndicator>().OnTriggerDestroy(boxCollider);
            
            Destroy(gameObject);
        }
        
        private void ChangeMaterial(Material material, Transform ttransform)
        {
            
            foreach (Transform child in ttransform)
            {
                Renderer rrenderer = child.GetComponent<Renderer>();
                
                if (rrenderer != null && rrenderer is not SpriteRenderer)
                {
                    Material[] newMaterials = new Material[rrenderer.materials.Length];
                    for (int i = 0; i < newMaterials.Length; i++)
                    {
                        newMaterials[i] = material;
                    }
                    rrenderer.materials = newMaterials;
                }
                
                if (child.childCount > 0)
                {
                    ChangeMaterial(material, child);
                }
            }
        }

        public void PauseBuilding()
        {
            
        }
        
        public void ResumeBuilding()
        {
            
        }
    }
}
