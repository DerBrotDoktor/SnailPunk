using System;
using Resources;
using Snails;
using UnityEngine;

namespace Buildings
{
    public readonly struct BuildingInfo
    {
        public readonly Building Building;
        public readonly BuildingData Data;
        
        public string Name => Data.BuildingName;
        public string Description => Data.BuildingDescription;
        public Sprite Icon => Data.BuildingIcon;
        
        public readonly int WorkerCount;
        public readonly int MaxWorkerCount;

        public readonly InventorySlot OutputSlot;
        public readonly InventorySlot InputSlot;
        
        public readonly float ProductionTime;

        public readonly Snail[] Snails;


        public BuildingInfo(Building building, BuildingData data, Snail[] snails, int maxWorkerCount)
        {
            Building = building;
            this.Data = data;
            Snails = snails;
            WorkerCount = snails.Length;
            MaxWorkerCount = maxWorkerCount;
            OutputSlot = new InventorySlot();
            InputSlot = new InventorySlot();
            ProductionTime = -1;
        }
        
        public BuildingInfo(Building building, BuildingData data, Snail[] snails, int maxWorkerCount, InventorySlot outputSlot)
        {
            Building = building;
            this.Data = data;
            Snails = snails;
            WorkerCount = snails.Length;
            MaxWorkerCount = maxWorkerCount;
            OutputSlot = outputSlot;
            InputSlot = new InventorySlot();
            ProductionTime = -1;
        }
        
        public BuildingInfo(Building building, BuildingData data, Snail[] snails, int maxWorkerCount, InventorySlot outputSlot, InventorySlot inputSlot)
        {
            Building = building;
            this.Data = data;
            Snails = snails;
            WorkerCount = snails.Length;
            MaxWorkerCount = maxWorkerCount;
            OutputSlot = outputSlot;
            InputSlot = inputSlot;
            ProductionTime = -1;
        }
        
        public BuildingInfo(Building building, BuildingData data, Snail[] snails, int maxWorkerCount, InventorySlot outputSlot, InventorySlot inputSlot, float productionTime)
        {
            Building = building;
            this.Data = data;
            Snails = snails;
            WorkerCount = snails.Length;
            MaxWorkerCount = maxWorkerCount;
            OutputSlot = outputSlot;
            InputSlot = inputSlot;
            ProductionTime = productionTime;
        }
    }
}
