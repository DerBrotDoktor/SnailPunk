using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TilemapGrid
{
    private Tilemap tilemap;
    private float cellSize;
    private Vector3 originPosition;
    
    public TilemapGrid(Tilemap tilemap, float cellSize, Vector3 originPosition)
    {
        this.tilemap = tilemap;
        this.cellSize = cellSize;
        this.originPosition = originPosition;
    }

    public Vector3 GetWorldPosition(int x, int y)
    {
        return new Vector3(x, y) * cellSize + originPosition;
    }

    public void GetXY(Vector3 worldPosition, out int x, out int y)
    {
        x = Mathf.FloorToInt((worldPosition - originPosition).x / cellSize);
        y = Mathf.FloorToInt((worldPosition - originPosition).y / cellSize);
    }

    public bool IsWalkable(Vector3Int position)
    {
        TileBase tile = tilemap.GetTile(position);
        // Check if the tile is null to determine if it's walkable or not
        return tile != null;
    }

    public Vector3Int WorldToCell(Vector3 worldPosition)
    {
        return tilemap.WorldToCell(worldPosition);
    }

    public bool HasTile(Vector3Int position)
    {
        return tilemap.HasTile(position);
    }
}