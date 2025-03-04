using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

namespace UI
{
    public class BuildBarButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] private BuildBar buildBar;
        [SerializeField] private string panelName;
        
        private const int FRAMES_AFTER_HOVER = 10;
        private int framesSinceHover = 0;
        
        private bool isHovering = false;
        
        public void OnPointerEnter(PointerEventData eventData)
        {
            buildBar.OnPanelHoverStarted(panelName);
            isHovering = true;
            framesSinceHover = 0;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            isHovering = false;
            StartCoroutine(StopHover());
        }

        private IEnumerator StopHover()
        {
            while (framesSinceHover < FRAMES_AFTER_HOVER)
            {
                yield return new WaitForEndOfFrame();
                framesSinceHover++;
            }

            if (!isHovering)
            {
                buildBar.OnPanelHoverStopped(panelName);
            }
        }
    }
}
