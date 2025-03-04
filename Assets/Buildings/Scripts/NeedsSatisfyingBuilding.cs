using Snails;
using UnityEngine;

namespace Buildings
{
    [RequireComponent(typeof(BoxCollider))]
    public class NeedsSatisfyingBuilding : MonoBehaviour
    {
        public SnailNeedType Need;
        public int AmountPerSecond = 1;
    }
}
