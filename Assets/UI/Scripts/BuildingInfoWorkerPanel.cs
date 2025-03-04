using System.Collections;
using System.Collections.Generic;
using Snails;
using TMPro;
using UI;
using UnityEngine;

public class BuildingInfoWorkerPanel : MonoBehaviour
{
    [SerializeField] private TMP_Text text;
    private Snail currentSnail;

    public void SetSnail(Snail snail)
    {
        currentSnail = snail;

        if (snail == null)
        {
            text.text = "";
            return;
        }

        text.text = snail.InfoData.Name;
    }

    public void OnClick()
    {
        FindObjectOfType<BuildingInfoPanel>().ClosePanel();
        
        var snailInfoPanel = FindObjectOfType<SnailInfoPanel>();
        snailInfoPanel.SetSnail(currentSnail);
        snailInfoPanel.OpenPanel();
        
    }
}
