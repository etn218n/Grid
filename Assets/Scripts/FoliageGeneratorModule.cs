using GridSystem;
using UnityEngine;
using Random = System.Random;

public class FoliageGeneratorModule : GridEngineModule
{
    private GridEngine engine;
    
    [SerializeField] private Sprite tree;
    [SerializeField] private Terrain empty;

    private Random random = new Random(0);
    private Rect2D treeRect2D;
    
    public override void OnStart(GridEngine engine)
    {
        this.engine = engine;
        
        this.treeRect2D = Extension.GetUVRect(tree);
        
        var foliageGrid = engine.FoliageGrid;
        
        foliageGrid.ForEachCoordinate(coordinate =>
        {
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
        });
        
        Grow();
    }
    
    private Terrain RaycastTerrain(Vector2Int coordinate)
    {
        var backgroundGrid = engine.BackgroundGrid;
        var foregroundGrid = engine.ForegroundGrid;
        
        var tile = foregroundGrid.TryGetTileAtCoordinate(coordinate);
            
        if (tile.Terrain != empty)
            return tile.Terrain;

        tile = backgroundGrid.TryGetTileAtCoordinate(coordinate);
            
        if (tile.Terrain != empty)
            return tile.Terrain;

        return null;
    }

    private void TryGrowTreeAt(Vector2Int coordinate, Terrain terrain)
    {
        float growChance = (random.Next() % 100) * terrain.Fertility;

        if (growChance > 80)
        {
            engine.FoliageGrid.SetTileUV(in treeRect2D, coordinate);
        }
                
    }
    
    public void Grow()
    {
        engine.FoliageGrid.SetUV(Rect2D.Zero);
            
        engine.FoliageGrid.ForEachCoordinate(coordinate =>
        {
            Tree(coordinate);
        });
    }

    private void Tree(Vector2Int coordinate)
    {
        var terrain = RaycastTerrain(coordinate);
            
        if (terrain != null)
            TryGrowTreeAt(coordinate, terrain);
    }
}