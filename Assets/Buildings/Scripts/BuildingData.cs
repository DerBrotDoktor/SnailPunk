using System;
using System.Collections.Generic;
using Resources;
using UnityEngine;

namespace Buildings
{
    [CreateAssetMenu(fileName = "BuildingData", menuName = "BuildingData")]
    public class BuildingData : ScriptableObject
    {
        [SerializeField] private string _buildingName;
        public string BuildingName => _buildingName;

        [SerializeField] private string _buildingDescription;
        public string BuildingDescription => _buildingDescription;

        [SerializeField] private Sprite _buildingIcon;
        public Sprite BuildingIcon => _buildingIcon;

        [SerializeField] private Building _buildingPrefab;
        public Building BuildingPrefab => _buildingPrefab;
        
        [SerializeField] private Vector3Int _buildingSize = new Vector3Int(1,1,1);
        public Vector3Int BuildingSize => _buildingSize;
    
        public Vector3 CenterPosition => new Vector3(BuildingSize.x / 2f, 0, BuildingSize.z / 2f);
        
        [Header("Street Position")]
        [SerializeField] private Vector3Int _internalStreetPosition = new Vector3Int(0,0,0);
        public Vector3Int InternalStreetPosition => _internalStreetPosition;
        
        [SerializeField] private Vector3Int _externalStreetPosition = new Vector3Int(0,0,-1);
        public Vector3Int ExternalStreetPosition => _externalStreetPosition;

        
        [SerializeField] private List<BuildingCost> _buildingCosts = new List<BuildingCost>();
        public List<BuildingCost> BuildingCosts => _buildingCosts;
    }

    [Serializable]
    public struct BuildingCost
    {
        public ResourceType resourceType;
        public int amount;
    }
}
