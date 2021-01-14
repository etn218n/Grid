using Optional;
using GridSystem;
using UnityEngine;

public class MovementMapModule : GridEngineModule
{
    private GridEngine engine;

    public override void OnStart(GridEngine engine)
    {
        this.engine = engine;
        
        engine.MovementGrid.ForEachTile(tile => tile.UpdateCost(CalculateMovementCost(tile.Coordinate)));
        engine.MovementGrid.ForEachTile(tile => PaintTileBasedOnMovementCost(tile));
    }

    private int CalculateMovementCost(Vector2Int coordinate)
    {
        var movementCostA = engine.ForegroundGrid.GetTileAt(coordinate).Map(tile => tile.Terrain.MovementCost);
        var movementCostB = engine.PlantGrid.GetTileAt(coordinate).Map<short>(tile =>
        {
            if (tile.IsCollidable)
                return 10;

            return 0;
        });

        var totalCost = movementCostA.ValueOr(0) + movementCostB.ValueOr(0);

        return Mathf.Clamp(totalCost, 0, 10);
    }

    private void PaintTileBasedOnMovementCost(MovementTile movementTile)
    {
        var movementCostPercentage = 1.1f - movementTile.MovementCost / 10.0f;
        
        var uvRect = new Rect2D(new Vector2(movementCostPercentage, 0),
                                new Vector2(movementCostPercentage, 0), 
                                new Vector2(movementCostPercentage, 0), 
                                new Vector2(movementCostPercentage, 0));
        
        movementTile.SetUVs(uvRect);
    }
}