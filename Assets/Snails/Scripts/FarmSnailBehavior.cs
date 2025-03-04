using System.Collections;
using Buildings;
using Plants;
using UnityEngine;

namespace Snails
{
    public class FarmSnailBehavior : SnailBehavior
    {
        [SerializeField] private int harvestAmount = 1;
        [SerializeField] private float timeToHarvest = 1f;
        public Farm Farm;

        private Plant targetResource;
        private bool isHarvesting = false;
        private bool walkToResource = false;

        private Storage targetStorage;
        private Field targetField;
        private bool walkToField = false;

        public void SetTargetPlant(Plant resource)
        {
            if (resource == null || !resource.TrySetTarget()) return;

            targetResource = resource;
            walkToResource = true;
            snail.SetDestination(resource.transform.position, true);
        }

        public void SetTargetStorage(Storage storage)
        {
            targetStorage = storage;
            snail.SetDestination(storage.GetSnailTargetPosition());
        }

        public override void OnTargetReached()
        {
            if (isHarvesting) return;

            if (walkToResource)
            {
                HandleResource();
            }
            else if (targetStorage != null)
            {
                HandleStorage();
            }
            else if (walkToField)
            {
                HandleField();
            }
            else
            {
                snail.NextTask();
            }
        }

        private void HandleResource()
        {
            if (targetResource == null)
            {
                snail.SetDestination(assignedBuilding.GetSnailTargetPosition(), true);
                walkToResource = false;
                return;
            }
            StartHarvesting();
        }

        private void HandleStorage()
        {
            if (targetStorage == null) return;

            targetStorage.InsertResource(snail.inventorySlot.resourceType, snail.inventorySlot.currentAmount);
            snail.inventorySlot.currentAmount = 0;
            targetStorage = null;
            snail.SetDestination(assignedBuilding.GetSnailTargetPosition());
        }

        private void HandleField()
        {
            if (targetField == null)
            {
                snail.SetDestination(assignedBuilding.GetSnailTargetPosition(), true);
                walkToField = false;
                return;
            }

            StartCoroutine(Plant());
        }

        private void StartHarvesting()
        {
            isHarvesting = true;
            walkToResource = false;
            StartCoroutine(Harvest());
        }

        private IEnumerator Harvest()
        {
            yield return new WaitForSeconds(targetResource.GetHarvestTime());

            if (targetResource != null)
            {
                snail.inventorySlot.currentAmount += targetResource.Harvest();
                targetResource = null;
            }

            isHarvesting = false;
            snail.SetDestination(assignedBuilding.GetSnailTargetPosition(), true);
        }

        protected override void SnailReached()
        {
            if (Farm == null)
            {
                Farm = assignedBuilding.GetComponent<Farm>();
            }

            Farm.SnailReached(this);
        }

        public void SetTargetField(Field field)
        {
            if (field == null) return;

            targetField = field;
            field.IsSnailTarget = true;
            walkToField = true;
            snail.SetDestination(field.transform.position, true);
        }

        private IEnumerator Plant()
        {
            if (targetField == null) yield break;

            walkToField = false;
            yield return new WaitForSeconds(targetField.timeToPlant);

            if (targetField != null)
            {
                targetField.Plant();
                targetField.IsSnailTarget = false;
                targetField = null;
            }

            snail.SetDestination(assignedBuilding.InternalStreetPosition, true);
        }
    }
}
