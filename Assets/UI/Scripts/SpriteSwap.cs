using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UI
{
    public class SpriteSwap : MonoBehaviour,IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] private Image[] images = Array.Empty<Image>();
        
        [SerializeField] private Sprite defaultSprite;
        [SerializeField] private Sprite hoveredSprite;
        
        public void OnPointerEnter(PointerEventData eventData)
        {
            foreach (var image in images)
            {
                image.sprite = hoveredSprite;
            }
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            foreach (var image in images)
            {
                image.sprite = defaultSprite;
            }
        }
    }
}
