using System;
using Buildings;

namespace Snails
{
    public struct SnailInfoData
    {
        public String Name;
        public int Age;
        public House House;
        public Building AssignedBuilding;

        public Action DataChanged;
        public Action NeedsChanged;

        public float WaterPercent;
        public float FoodPercent;
        public float SleepPercent;
    }
}
