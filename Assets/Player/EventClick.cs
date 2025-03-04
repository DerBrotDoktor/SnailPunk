using Buildings;
using Snails;
using UI;
using UnityEngine;
using UnityEngine.EventSystems;

public class EventClick : MonoBehaviour, IPointerClickHandler
{
    public void OnPointerClick(PointerEventData eventData)
    {
        if(gameObject.TryGetComponent<Building>(out var building))
        {
            UIController.BuildingClicked(building);
        }
        else if(gameObject.TryGetComponent<Snail>(out var snail))
        {
            UIController.SnailClicked(snail);
        }
    }
}
