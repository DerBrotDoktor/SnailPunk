using System.Collections.Generic;
using Plants;
using Resources;
using Snails;
using UnityEngine;

namespace Buildings
{
    public class Forester : Building
    {
        [SerializeField] private GameObject rangeCircle;
        [SerializeField] private float radius = 2f;
        [SerializeField] private ResourceType harvestResourceType;
        [SerializeField] private LayerMask layerMask;

        private List<PlantSnailBehavior> snailQueue = new List<PlantSnailBehavior>();
        
        private Field FindNextField()
        {
            float minDistance = float.MaxValue;
            Field closestField = null;
            
            Collider[] hitColliders = Physics.OverlapSphere(transform.position, radius, layerMask);
            foreach (var hitCollider in hitColliders)
            {
                if (hitCollider.gameObject.TryGetComponent<Field>(out var field) && !field.IsSnailTarget && field.CanBePlanted())
                {
                    if(field.ResourceType != harvestResourceType) continue;
                    
                    float distance = Vector3.Distance(transform.position, field.transform.position);
                    
                    if (distance <= radius && distance < minDistance)
                    {
                        minDistance = distance;
                        closestField = field;
                    }
                }
            }
            return closestField;
        }


        public override void OnBuild(Vector3Int colliderSize, Vector3 internalStreetPosition, BuildingData buildingData, bool construction = true)
        {
            base.OnBuild(colliderSize, internalStreetPosition, buildingData, construction);
            rangeCircle.SetActive(false);
            print("Build Resource Harvester");
        }

        public override void OnHologram(Material hologramMaterial)
        {
            base.OnHologram(hologramMaterial);
            rangeCircle.transform.localScale = new Vector3(radius*2,radius*2,1);
            rangeCircle.SetActive(true);
        }

        public override void AddSnail(SnailBehavior snail)
        {
            if (snail.gameObject.TryGetComponent<PlantSnailBehavior>(out PlantSnailBehavior hSnail))
            {
                base.AddSnail(hSnail);
                snailQueue.Add(hSnail);
            }
        }

        public override void RemoveSnail(SnailBehavior snail)
        {
            base.RemoveSnail(snail);
            snailQueue.Remove(snail.GetComponent<PlantSnailBehavior>());
        }

        public override void Update()
        {
            base.Update();

            if (snailQueue.Count > 0)
            {
                PlantSnailBehavior snail = snailQueue[0];
                Field field = FindNextField();

                if (field != null)
                {
                    snail.SetTargetField(field);
                    snailQueue.RemoveAt(0);
                }
            }
        }

        public override void SnailReached(SnailBehavior snail)
        {
            base.SnailReached(snail);
            snailQueue.Add(snail.GetComponent<PlantSnailBehavior>());
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.magenta;
            Gizmos.DrawWireSphere(transform.position, radius);
        }

        public override void DestroyBuilding()
        {
            base.DestroyBuilding();
            // for (int i = 0; i < requestedWorkerCount; i++)
            // {
            //     DecreaseWorkerCount();
            // }
            //
            // SnailBehavior[] tSnails = new SnailBehavior[snails.Count];
            // snails.CopyTo(tSnails); 
            // foreach (var snail in tSnails)
            // {
            //     snail.shouldBeUnAssigned = true;
            //     if (snailQueue.Contains(snail.gameObject.GetComponent<PlantSnailBehavior>()))
            //     {
            //         SnailReached(snail);
            //     }
            // }
        }
    }
}
