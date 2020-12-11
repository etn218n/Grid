﻿using GridSystem;
using UnityEngine;

public class TerrainTile : BaseTile<TerrainTile>
{
    private Terrain terrain;
    public Terrain Terrain => terrain;

    public TerrainTile(Grid<TerrainTile> ownerGrid, Chunk ownerChunk, Vector2Int coordinate, Vector2Int localCoordinate) : 
        base(ownerGrid, ownerChunk, coordinate, localCoordinate)
    {
    }

    public void SetTerrain(Terrain terrain)
    {
        id = terrain == null ? 0 : terrain.GetHashCode();
        
        this.terrain = terrain;
    }
    
    public void Paint(Terrain terrain)
    {
        if (terrain == null)
            return;

        SetTerrain(terrain);
        SetUVs(in terrain.SpriteRect2D);
    }

    public void Paint()
    {
        if (terrain == null)
            return;
        
        SetUVs(in terrain.SpriteRect2D);
    }

    public void ApplyRule()
    {
        if (terrain == null)
            return;

        ref readonly var uvRect = ref terrain.RuleResolver.Output(this);
        
        if (!uvRect.Equals(Rect2D.Zero))
            SetUVs(in uvRect);
    }
}