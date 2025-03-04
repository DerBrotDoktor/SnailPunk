using UnityEngine;

namespace Buildings
{
    public class DecorativeBuilding : Building
    {
        protected override void Awake()
        {
            base.Awake();
            maxWorkerCount = 0;
        }

        public override void OnBuild(Vector3Int colliderSize, Vector3 internalStreetPosition, BuildingData buildingData, bool construction = true)
        {
            base.OnBuild(colliderSize, internalStreetPosition, buildingData, false);
        }
    }
}
