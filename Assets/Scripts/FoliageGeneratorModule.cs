using GridSystem;
using UnityEngine;
using System.Collections.Generic;
using Random = UnityEngine.Random;

public class FoliageGeneratorModule : GridEngineModule
{
    private GridEngine engine;
    
    [SerializeField] private Terrain empty;
    [SerializeField] private float growChance;
    
    [SerializeField] private List<Foliage> foliages = new List<Foliage>();

    public override void OnStart(GridEngine engine)
    {
        this.engine = engine;

        engine.FoliageGrid.ForEachCoordinate(coordinate => AdjustTileVerticesAt(coordinate));
        
        GenerateFoliage();
    }

    private void AdjustTileVerticesAt(Vector2Int coordinate)
    {
        var foliageGrid = engine.FoliageGrid;
        
        Rect3D verticesRect = foliageGrid.GetTileVertices(coordinate);

        Vector3 bottomLeft  = verticesRect.BottomLeft;
        Vector3 topLeft     = verticesRect.TopLeft;
        Vector3 bottomRight = verticesRect.BottomRight;
        Vector3 topRight    = verticesRect.TopRight;
            
        topLeft.y  += foliageGrid.TileSize;
        topRight.y += foliageGrid.TileSize;

        float zValue = (bottomLeft.y) * 0.0001f;
        bottomLeft.z  = zValue;
        topLeft.z     = zValue;
        bottomRight.z = zValue;
        topRight.z    = zValue;
 
        foliageGrid.SetTileVertices(new Rect3D(bottomLeft, topLeft, bottomRight, topRight), coordinate);
    }
    
    public void GenerateFoliage()
    {
        engine.FoliageGrid.SetUV(Rect2D.Zero);
        engine.FoliageGrid.ForEachCoordinate(coordinate => TryGrowTreeAt(coordinate));
    }
    
    private Terrain RaycastTerrain(Vector2Int coordinate)
    {
        var backgroundGrid = engine.BackgroundGrid;
        var foregroundGrid = engine.ForegroundGrid;
        
        var tile = foregroundGrid.TryGetTileAtCoordinate(coordinate);
            
        if (tile.Terrain != empty)
            return tile.Terrain;

        return backgroundGrid.TryGetTileAtCoordinate(coordinate).Terrain;
    }
    
    private void TryGrowTreeAt(Vector2Int coordinate)
    {
        var terrain = RaycastTerrain(coordinate);

        if (terrain.Fertility != 0)
        {
            float growValue = Random.Range(0.0f, 1.0f);

            if (growValue <= growChance)
            {
                int randomIndex = Random.Range(0, foliages.Count);
                engine.FoliageGrid.SetTileUV(in foliages[randomIndex].SpriteRect2D, coordinate);
            }
        }
    }
}