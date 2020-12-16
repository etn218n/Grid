using GridSystem;
using UnityEngine;

public class TerrainTile : BaseTile<TerrainTile>
{
    private Terrain terrain;
    public Terrain Terrain => terrain;

    public TerrainTile(Grid<TerrainTile> ownerGrid, Vector2Int coordinate) : base(ownerGrid, coordinate)
    {
    }

    public void SetTerrain(Terrain terrain)
    {
        this.terrain = terrain;
    }

    public override bool SameTileCategory(TerrainTile otherTile)
    {
        return terrain == otherTile.Terrain;
    }

    public void Paint(Terrain terrain)
    {
        if (terrain == null)
        {
            SetUVs(in Rect2D.Zero);
            return;
        }

        SetTerrain(terrain);
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
