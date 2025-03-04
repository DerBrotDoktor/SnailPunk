using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class ToolTipBuildingCost : MonoBehaviour
    {
        
        [SerializeField] private Image icon;
        [SerializeField] private TMP_Text text;


        public void Initialize(Sprite sprite, string text)
        {
            icon.sprite = sprite;
            this.text.text = text;
        }
    }
}
