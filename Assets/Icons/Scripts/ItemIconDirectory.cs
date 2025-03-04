using System;
using System.Collections.Generic;
using Resources;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemIconDirectory", menuName = "IconDirectory/ItemIconDirectory")]
public class ItemIconDirectory : ScriptableObject
{
    [SerializeField] private List<Icon> icons = new List<Icon>();
    public List<Icon> Icons => icons;

    public Sprite GetSprite(ResourceType resourceType)
    {
        return icons.Find(icon => icon.Name == resourceType).Sprite;
    }
}

[Serializable]
public struct Icon
{
    public ResourceType Name;
    public Sprite Sprite;
}
