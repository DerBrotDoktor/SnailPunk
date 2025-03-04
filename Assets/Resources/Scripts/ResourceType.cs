using System;

namespace Resources
{
    [Serializable]
    public enum ResourceType
    {
        None,
    
        Log,
        Plank,
        Gear,
        
        Water = 10,

        Lettuce = 20,
        Tomato,
        Potato,
        
        Food = 30,
        Drink,

        Snail = 100,
        HouseSpace = 200
    }
}
