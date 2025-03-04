using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;

namespace BuildingSystem
{
    public class StreetHandler : MonoBehaviour
    {
        [SerializeField] private Tilemap tileMap;
        [SerializeField] private RuleTile streetTile;
        
        public void PlaceStreet(Vector3 position)
        {
            tileMap.SetTile(tileMap.WorldToCell(position), streetTile);
        }

        public void DestroyStreet(Vector3 position)
        {
            if (!HasTile(position)) return;
            
            print("Destroy Street at: " + GetTilePosition(position));
            
            GameObject tile = tileMap.GetInstantiatedObject(tileMap.WorldToCell(position));
            if (tile.TryGetComponent<Collider>(out var coll))
            {
                FindObjectOfType<MouseIndicator>().OnTriggerDestroy(coll);
            }
            tileMap.SetTile(tileMap.WorldToCell(position), null);
        }

        public bool HasTile(Vector3 position)
        {
            return tileMap.HasTile(tileMap.WorldToCell(position));
        }
        
        public Vector3Int GetTilePosition(Vector3 position)
        {
            return tileMap.WorldToCell(position);
        }

        public Vector3 GetWorldPosition(Vector3Int position)
        {
            return tileMap.CellToWorld(position);
        }
    }
}
