using Optional;
using GridSystem;
using UnityEngine;

public class MovementMapModule : GridEngineModule
{
    private GridEngine engine;

    public override void OnStart(GridEngine engine)
    {
        this.engine = engine;

        engine.MovementGrid.ForEachTile(tile => PaintTileBasedOnMovementCost(engine.PlantGrid, tile));
        engine.MovementGrid.ForEachTile(tile => PaintTileBasedOnMovementCost(engine.ForegroundGrid, tile));
    }

    private void PaintTileBasedOnMovementCost<T>(Grid<T> grid, MovementTile movementTile) where T : BaseTile<T>
    {
        var gradient = GradientFromMovementCost(grid, movementTile.Coordinate);

        gradient.MatchSome(value =>
        {
            var uvRect = new Rect2D(new Vector2(value, 0), 
                                    new Vector2(value, 0), 
                                    new Vector2(value, 0), 
                                    new Vector2(value, 0));
        
            movementTile.SetUVs(uvRect);
        });
    }

    private Option<float> GradientFromMovementCost<T>(Grid<T> grid, Vector2Int coordinate) where T : BaseTile<T>
    {
        return grid.GetTileAt(coordinate).Filter(tile => tile.IsCollidable).Map(tile => 0.1f);
    }
}