using GridSystem;
using UnityEngine;

public class MovementMapModule : GridEngineModule
{
    private GridEngine engine;
    
    [SerializeField] private Terrain empty;

    public override void OnStart(GridEngine engine)
    {
        this.engine = engine;

        engine.MovementGrid.ForEachCoordinate(coordinate => PaintTileBasedOnMovementCost(coordinate));
    }

    private void PaintTileBasedOnMovementCost(Vector2Int coordinate)
    {
        float gradient = MovementCostToColorGradient(coordinate);
        
        var uvRect = new Rect2D(new Vector2(gradient, 0), 
                                new Vector2(gradient, 0), 
                                new Vector2(gradient, 0), 
                                new Vector2(gradient, 0));
        
        engine.MovementGrid.SetTileUV(uvRect, coordinate);
    }

    private float MovementCostToColorGradient(Vector2Int coordinate)
    {
        var backgroundGrid = engine.BackgroundGrid;
        var foregroundGrid = engine.ForegroundGrid;
        
        var tile = foregroundGrid.TryGetTileAtCoordinate(coordinate);

        if (tile.Terrain != empty)
            return 1 - tile.Terrain.MovementCost / short.MaxValue;
        
        tile = backgroundGrid.TryGetTileAtCoordinate(coordinate);
        
        return 1 - tile.Terrain.MovementCost / short.MaxValue;
    }
}