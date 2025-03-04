using System;
using UnityEngine;

namespace Snails
{
    [Serializable]
    public struct SnailNeed
    {
        public SnailNeedType Type;
        public Sprite Icon;
    
        public int Amount;
        public int MaxAmount;
        
        public int SatisfyThreshold;
        public int ComplainThreshold;
        
        [Range(0,100)]
        public int SatisfyBeforeThresholdProbability;

        public int DecreasePerSecond;
        public int SatisfyAmount;

        public string DeathMessage;
        
        [NonSerialized] public bool CurrentlySatisfying;
        
        public bool CurrentlyComplaining => Amount <= ComplainThreshold;

        public void Tick()
        {
            if (CurrentlySatisfying)
            {
                Amount += SatisfyAmount * Game.TimeScale;
            }
            else
            {
                Amount -= DecreasePerSecond * Game.TimeScale;
            }
        }
    }

    public enum SnailNeedType
    {
        None,
    
        Sleep,
        Food,
        Water
    }
}