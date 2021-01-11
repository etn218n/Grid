using GridSystem;
using Optional;
using UnityEngine;

public class MovementMapModule : GridEngineModule
{
    private GridEngine engine;
    
    [SerializeField] private Terrain empty;

    public override void OnStart(GridEngine engine)
    {
        this.engine = engine;
        engine.MovementGrid.ForEachCoordinate(PaintTileBasedOnMovementCost);
    }

    private void PaintTileBasedOnMovementCost(Vector2Int coordinate)
    {
        var gradient = GradientFromMovementCost(engine.BackgroundGrid, coordinate)
                        .Else(GradientFromMovementCost(engine.ForegroundGrid, coordinate));
        
        gradient.MatchSome(value =>
        {
            var uvRect = new Rect2D(new Vector2(value, 0), 
                                    new Vector2(value, 0), 
                                    new Vector2(value, 0), 
                                    new Vector2(value, 0));
        
            engine.MovementGrid.SetTileUVsAt(coordinate, uvRect);
        });
    }

    private Option<float> GradientFromMovementCost(Grid<TerrainTile> terrainGrid, Vector2Int coordinate)
    {
        return terrainGrid.GetTileAt(coordinate).Filter(tile => tile.Terrain != empty)
                                                .Map(tile => 1.0f - (float)tile.Terrain.MovementCost / short.MaxValue);
    }
}