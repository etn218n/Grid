using UnityEngine;
using System.Collections.Generic;
using Random = UnityEngine.Random;

public class PlantSeederModule : GridEngineModule
{
    private GridEngine engine;
    
    [SerializeField] private Terrain empty;
    [SerializeField] private float growChance;
    [SerializeField] private List<PlantBlueprint> plantBlueprints = new List<PlantBlueprint>();

    public override void OnStart(GridEngine engine)
    {
        this.engine = engine;

        SeedPlants();
    }

    public void SeedPlants()
    {
        engine.PlantGrid.ForEachTile(tile => TryGrowTreeAt(tile));
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
    
    private void TryGrowTreeAt(PlantTile tile)
    {
        var terrain = RaycastTerrain(tile.Coordinate);

        if (terrain.Fertility != 0)
        {
            float growValue = Random.Range(0.0f, 1.0f);

            if (growValue <= Mathf.Clamp01(growChance))
            {
                var randomIndex = Random.Range(0, plantBlueprints.Count);
                var choosenPlantBlueprint = plantBlueprints[randomIndex];

                tile.Fertilize(terrain.Fertility);
                tile.Seed(choosenPlantBlueprint.CreatePlant());
            }
        }
    }
}