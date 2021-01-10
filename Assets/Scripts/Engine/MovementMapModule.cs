using GridSystem;
using UnityEngine;

public class MovementMapModule : GridEngineModule
{
    private GridEngine engine;
    
    [SerializeField] private Terrain empty;

    public override void OnStart(GridEngine engine)
    {
        this.engine = engine;

        SetupMovementGrid(engine);
        //engine.MovementGrid.ForEachCoordinate(coordinate => PaintTileBasedOnMovementCost(coordinate));
    }

    private void PaintTileBasedOnMovementCost(Vector2Int coordinate)
    {
        float gradient = MovementCostToColorGradient(coordinate);
        
        var uvRect = new Rect2D(new Vector2(gradient, 0), 
                                new Vector2(gradient, 0), 
                                new Vector2(gradient, 0), 
                                new Vector2(gradient, 0));
        
        engine.MovementGrid.SetTileUVsAt(coordinate, uvRect);
    }

    public void SetupMovementGrid(GridEngine engine)
    {
        
    }

    private float MovementCostToColorGradient(Vector2Int coordinate)
    {
        var backgroundGrid = engine.BackgroundGrid;
        var foregroundGrid = engine.ForegroundGrid;
        
        var tile = foregroundGrid.TryGetTileAt(coordinate);

        if (tile.Terrain != empty)
            return 1 - tile.Terrain.MovementCost / short.MaxValue;
        
        tile = backgroundGrid.TryGetTileAt(coordinate);
        
        return 1 - tile.Terrain.MovementCost / short.MaxValue;
    }
}