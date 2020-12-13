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

        engine.FoliageGrid.ForEachCoordinate(coordinate => AdjustTileDepthAt(coordinate));
        
        GenerateFoliage();
    }

    private void AdjustTileDepthAt(Vector2Int coordinate)
    {
        var foliageGrid = engine.FoliageGrid;
        
        Rect3D verticesRect = foliageGrid.GetTileVertices(coordinate);

        Vector3 bottomLeft  = verticesRect.BottomLeft;
        Vector3 topLeft     = verticesRect.TopLeft;
        Vector3 bottomRight = verticesRect.BottomRight;
        Vector3 topRight    = verticesRect.TopRight;

        float zValue  = (bottomLeft.y) * 0.0001f;
        topLeft.z     = zValue;
        topRight.z    = zValue;
        bottomLeft.z  = zValue;
        bottomRight.z = zValue;

        foliageGrid.SetTileVertices(new Rect3D(bottomLeft, topLeft, bottomRight, topRight), coordinate);
    }

    private void AdjustTileDimensionAt(Vector2Int coordinate, int width, int height)
    {
        var foliageGrid = engine.FoliageGrid;
        
        Rect3D verticesRect = foliageGrid.GetTileVertices(coordinate);

        Vector3 bottomLeft  = verticesRect.BottomLeft;
        Vector3 topLeft     = verticesRect.TopLeft;
        Vector3 bottomRight = verticesRect.BottomRight;
        Vector3 topRight    = verticesRect.TopRight;

        float tileWidth  = (width  * foliageGrid.TileSize) - foliageGrid.TileSize;
        float tileHeight = (height * foliageGrid.TileSize) - foliageGrid.TileSize;

        topRight.x    += tileWidth;
        bottomRight.x += tileWidth;

        topLeft.y  += tileHeight;
        topRight.y += tileHeight;

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

            if (growValue <= Mathf.Clamp01(growChance))
            {
                int randomIndex = Random.Range(0, foliages.Count);

                Foliage choosenFoliage = foliages[randomIndex];
                
                AdjustTileDimensionAt(coordinate, choosenFoliage.Width, choosenFoliage.Height);

                engine.FoliageGrid.SetTileUV(in choosenFoliage.SpriteRect2D, coordinate);
            }
        }
    }
}