using System.Collections;
using Plants;
using UnityEngine;

namespace Snails
{
    public class PlantSnailBehavior: SnailBehavior
    {
        private Field targetField;
        private bool walkToField = false;
        
        public void SetTargetField(Field field)
        {
            targetField = field;
            field.IsSnailTarget = true;
            walkToField = true;
            snail.SetDestination(field.transform.position,true);
        }
        
        public override void OnTargetReached()
        {
            if (walkToField)
            {
                if (targetField == null)
                {
                    snail.SetDestination(assignedBuilding.InternalStreetPosition, true);
                    walkToField = false;
                    return;
                }
                StartCoroutine(Plant());
            }
            else
            {
                snail.NextTask();
            }
        }

        private IEnumerator Plant()
        {
            walkToField = false;
            if(targetField == null) yield break;
            
            yield return new WaitForSeconds(targetField.timeToPlant);
            if(targetField != null)
            {
                targetField.Plant();
                targetField.IsSnailTarget = false;
                targetField = null;
            }
            
            snail.SetDestination(assignedBuilding.InternalStreetPosition, true);
        }
    }
}