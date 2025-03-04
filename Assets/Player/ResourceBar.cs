using System.Collections.Generic;
using Resources;
using TMPro;
using UnityEngine;

public class ResourceBar : MonoBehaviour
{
    [SerializeField] private List<ResourceBarStruct> resourceBarStructs;
    private Dictionary<ResourceType, TMP_Text> resourceTexts = new Dictionary<ResourceType, TMP_Text>();

    private void Awake()
    {
        foreach (var item in resourceBarStructs)
        {
            resourceTexts.Add(item.resourceType, item.text);
        }

        foreach (var resourceText in resourceTexts)
        {
            resourceText.Value.text = "0";
        }
    }

    private void OnEnable()
    {
        GlobalInventory.ResourceChanged += OnResourceChanged;

        foreach (var resourceText in resourceTexts)
        {
            resourceText.Value.text = GlobalInventory.Instance.GetResource(resourceText.Key).ToString();
        }
    }
    
    private void OnDisable()
    {
        GlobalInventory.ResourceChanged -= OnResourceChanged;
    }

    void OnResourceChanged(ResourceType resourceType, int amount)
    {
        if (resourceTexts.ContainsKey(resourceType))
        {
            resourceTexts[resourceType].text = amount.ToString();
        }
    }
}
