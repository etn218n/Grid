using MayBe;
using UnityEngine;
using System.Collections.Generic;
using Random = UnityEngine.Random;

public class PlantSeederModule : GridEngineModule
{
    private GridEngine engine;
    
    [SerializeField] private float growChance;
    [SerializeField] private List<PlantBlueprint> plantBlueprints = new List<PlantBlueprint>();

    public override void OnStart(GridEngine engine)
    {
        this.engine = engine;
        
        engine.PlantGrid.ForEachTile(TryGrowTree);
    }
    
    private Maybe<Terrain> RaycastTerrain(Vector2Int coordinate)
    {
        return engine.ForegroundGrid.GetTileAt(coordinate).Map(tile => tile.Terrain);
    }
    
    private void TryGrowTree(PlantTile tile)
    {
        RaycastTerrain(tile.Coordinate).Filter(terrain => terrain.Fertility != 0)
                                       .MatchSome(terrain => Seed(tile, terrain.Fertility));
    }
    
    public void Seed(PlantTile tile, float fertility)
    {
        var growValue = Random.Range(0.0f, 1.0f);

        if (growValue <= Mathf.Clamp01(growChance))
        {
            var randomIndex = Random.Range(0, plantBlueprints.Count);
            var choosenPlantBlueprint = plantBlueprints[randomIndex];

            tile.Fertilize(fertility);
            tile.Seed(choosenPlantBlueprint.CreatePlant());
        }
    }
}