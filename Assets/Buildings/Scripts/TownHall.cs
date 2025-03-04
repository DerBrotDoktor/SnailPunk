using System;
using System.Collections.Generic;
using BuildingSystem;
using Pathfinding;
using Resources;
using Snails;
using UI;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

namespace Buildings
{
    public class TownHall : Building
    {
        [SerializeField] private BuildingData _buildingData;
        [SerializeField] private StreetBuilder streetBuilder;

        [SerializeField] private List<Building> buildingsWithoutWorker = new List<Building>(); 
        [SerializeField] private List<Snail> unAssignedSnails = new List<Snail>();

        [SerializeField] private int spawnWorkerCount = 1;

        [SerializeField] private List<ConstructionSnailBehavior> snailBehaviours = new List<ConstructionSnailBehavior>();
        [SerializeField] private List<ConstructionSite> constructionSites = new List<ConstructionSite>();

        private Pathfinder pathfinder;

        [Header("Unemployed Snails")]
        [SerializeField, Range(0,100f)] private float unemployedSnailWalkProbability = 0.01f;
        [SerializeField] private int unemployedSnailMinWalkDistance = 1;
        [SerializeField] private int unemployedSnailMaxWalkDistance = 10;
        
        protected override void Awake()
        {
            base.Awake();
            
            Vector3 pos = transform.position - new Vector3(2,0,2);
            streetBuilder.TryPlaceStreet(pos + _buildingData.InternalStreetPosition);
            streetBuilder.TryPlaceStreet(pos + _buildingData.ExternalStreetPosition);
            OnBuild(_buildingData.BuildingSize,  pos + _buildingData.InternalStreetPosition, _buildingData);
            OnConstructed();

           pathfinder = FindObjectOfType<Pathfinder>();
        }

        private void Start()
        {
            Vector3 pos = transform.position - new Vector3(2,0,2);
            
            for (int i = 0; i < spawnWorkerCount; i++)
            {
                SpawnSnail(pos + _buildingData.ExternalStreetPosition, false);
            }
        }

        public void SpawnSnail(Vector3 position, bool message = true)
        {
            Snail snail = FindObjectOfType<SnailSpawner>().SpawnSnailAtPosition(position);
            unAssignedSnails.Add(snail.GetComponent<Snail>());
            
            GlobalInventory.Instance.AddResource(ResourceType.Snail, 1);
            
            if (message)
            {
                FindObjectOfType<MessageInfoPanel>().NewMessage(snail.InfoData.Name + " was born");
            }
        }

        public void GetMoreWorkers(Building building)
        {
            if (!buildingsWithoutWorker.Contains(building))
            {
                buildingsWithoutWorker.Add(building);
            }
        }

        public override void Update()
        {
            base.Update();
            
            UpdateWorkers();
            UpdateUnAssignedSnails();
        }

        private void UpdateWorkers()
        {
            if(snailBehaviours.Count <= 0) return;

            foreach (var snail in snailBehaviours)
            {
                if (snail.IsConstructionSiteAssigned || snail.snail.IsMoving || snail.snail.IsDead) continue;

                snail.snail.NextTask();
            }
        }

        public void GetNextTask(ConstructionSnailBehavior snailBehavior)
        {
            if (snailBehavior.AssignedConstructionSite != null)
            {
                if (snailBehavior.AssignedConstructionSite.HasTask())
                {
                    snailBehavior.AssignedConstructionSite.GetTask(snailBehavior);
                    return;
                }
            }
            
            snailBehavior.UnAssignConstructionSite();
            
            foreach (var site in constructionSites)
            {
                if (!site.HasTask()) continue;
                
                var path = pathfinder.FindPath(snailBehavior.transform.position, site.GetSnailTargetPosition());
                
                if (path.Count <= 0) continue;
                
                snailBehavior.AssignConstructionSite(site);
                site.GetTask(snailBehavior);
                
                return;
            }
            
            snailBehavior.snail.SetDestination(GetSnailTargetPosition());
        }

        
        private void UpdateUnAssignedSnails()
        {
            if (unAssignedSnails.Count <= 0) return;

            List<Snail> removeSnails = new List<Snail>();

            foreach (var snail in unAssignedSnails)
            {
                bool assigned = false;

                for (int i = 0; i < buildingsWithoutWorker.Count; i++)
                {
                    Vector3 targetPosition = buildingsWithoutWorker[i].GetSnailTargetPosition();
                    if (pathfinder.FindPath(snail.transform.position, targetPosition).Count > 0)
                    {
                        snail.AssignBuilding(buildingsWithoutWorker[i]);
                        buildingsWithoutWorker[i].sendWorkerCount++;

                        if (buildingsWithoutWorker[i].requestedWorkerCount <= buildingsWithoutWorker[i].sendWorkerCount)
                        {
                            buildingsWithoutWorker.RemoveAt(i);
                            i--;
                        }

                        removeSnails.Add(snail);
                        assigned = true;
                        break;
                    }
                }

                if (!assigned)
                {
                    if (!snail.IsDead && !snail.IsMoving && !snail.SatisfyingNeeds)
                    {
                        snail.NextTask();
                        if (Random.Range(0, 100f) < unemployedSnailWalkProbability)
                        {
                            Vector3 randomTarget = pathfinder.GetRandomTarget(snail.transform.position, unemployedSnailMinWalkDistance, unemployedSnailMaxWalkDistance);
                            snail.SetDestination(randomTarget);
                        }
                    }
                }
            }

            foreach (var removeSnail in removeSnails)
            {
                unAssignedSnails.Remove(removeSnail);
            }
        }

        
        public override void OnBuild(Vector3Int colliderSize, Vector3 internalStreetPosition, BuildingData buildingData, bool construction = true)
        {
            base.OnBuild(colliderSize, internalStreetPosition, buildingData, false);
        }

        public void UnAssignSnail(Snail snail)
        {
            unAssignedSnails.Add(snail);
            snail.ChangeType(SnailType.None);
            snail.AssignBuilding(null);
        }

        public override void DestroyBuilding()
        {
            return;
        }
        
        public void AddConstructionSite(ConstructionSite constructionSite)
        {
            constructionSites.Add(constructionSite);
        }

        public void RemoveConstructionSite(ConstructionSite constructionSite)
        {
            if (!constructionSites.Contains(constructionSite)) return;

            foreach (var snail in snailBehaviours)
            {
                if (snail.IsConstructionSiteAssigned && snail.AssignedConstructionSite == constructionSite)
                {
                    snail.UnAssignConstructionSite();
                    snail.GetNextTask();
                }
            }
            
            constructionSites.Remove(constructionSite);
        }

        public override void AddSnail(SnailBehavior snail)
        {
            base.AddSnail(snail);
            snailBehaviours.Add(snail.gameObject.GetComponent<ConstructionSnailBehavior>());
        }

        public override void RemoveSnail(SnailBehavior snail)
        {
            base.RemoveSnail(snail);
            snailBehaviours.Remove(snail.gameObject.GetComponent<ConstructionSnailBehavior>());
        }
    }
}
