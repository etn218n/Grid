using GridSystem;
using UnityEngine;
using Random = UnityEngine.Random;

public class FoliageGeneratorModule : GridEngineModule
{
    private GridEngine engine;
    
    [SerializeField] private Sprite tree;
    [SerializeField] private Terrain empty;
    
    private Rect2D treeRect2D;

    public override void OnStart(GridEngine engine)
    {
        this.engine = engine;
        this.treeRect2D = Extension.GetUVRect(tree);

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

        if (terrain != null && terrain.GrowChance != 0)
        {
            float growValue = Random.Range(0.0f, 1.0f);

            if (growValue <= terrain.GrowChance)
            {
                engine.FoliageGrid.SetTileUV(in treeRect2D, coordinate);
            }
        }
    }
}