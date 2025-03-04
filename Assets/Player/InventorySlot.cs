using System;
using Resources;

[Serializable]
public struct InventorySlot
{
    public ResourceType resourceType;
    public int currentAmount;
    public int maxAmount;
        
    public readonly int RemainingSpace => maxAmount - currentAmount;

    public InventorySlot(ResourceType resourceType, int maxAmount)
    {
        this.resourceType = resourceType;
        this.currentAmount = 0;
        this.maxAmount = maxAmount;
    }
}