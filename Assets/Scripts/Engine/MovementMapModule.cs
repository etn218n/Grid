using GridSystem;
using UnityEngine;

public class MovementMapModule : GridEngineModule
{
    private GridEngine engine;

    public override void OnStart(GridEngine engine)
    {
        this.engine = engine;

        MovementTile.GenerateUVRectFromCost = MovementCostToUVRect;
        
        engine.MovementGrid.ForEachTile(tile => tile.UpdateCost(CalculateMovementCost(tile.Coordinate)));
        //engine.MovementGrid.ForEachTile(tile => PaintTileBasedOnMovementCost(tile));
    }

    private int CalculateMovementCost(Vector2Int coordinate)
    {
        var movementCostA = engine.ForegroundGrid.GetTileAt(coordinate).Map(tile => tile.Terrain.MovementCost);
        var movementCostB = engine.PlantGrid.GetTileAt(coordinate).Filter(tile => tile.IsCollidable).Map(tile => Movement.MaxCost);

        var totalCost = movementCostA.ValueOr(0) + movementCostB.ValueOr(0);

        return Mathf.Clamp(totalCost, Movement.MinCost, Movement.MaxCost);
    }

    private Rect2D MovementCostToUVRect(int movementCost)
    {
        var movementCostPercentage = 1.1f - (float)movementCost / Movement.MaxCost;
        
        var uvRect = new Rect2D(new Vector2(movementCostPercentage, 0),
                                new Vector2(movementCostPercentage, 0), 
                                new Vector2(movementCostPercentage, 0), 
                                new Vector2(movementCostPercentage, 0));

        return uvRect;
    }

    private void PaintTileBasedOnMovementCost(MovementTile movementTile)
    {
        var movementCostPercentage = 1.1f - (float)movementTile.MovementCost / Movement.MaxCost;
        
        var uvRect = new Rect2D(new Vector2(movementCostPercentage, 0),
                                new Vector2(movementCostPercentage, 0), 
                                new Vector2(movementCostPercentage, 0), 
                                new Vector2(movementCostPercentage, 0));
        
        movementTile.SetUVs(uvRect);
    }
}