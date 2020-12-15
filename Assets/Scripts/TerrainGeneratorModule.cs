using Sirenix.OdinInspector;
using UnityEngine;
using Grid = GridSystem.Grid;

public class TerrainGeneratorModule : GridEngineModule
{
    [Header("Terrain Setting")] 
    [SerializeField] private MapSetting basegroundMap;
    [SerializeField] private MapSetting backgroundMap;
    [SerializeField] private MapSetting foregroundMap;

    private GridEngine engine;
    
    [ShowInInspector]
    [PropertySpace(4)]
    [PropertyRange(0, 100)]
    public float Scale
    {
        get => scale;
        set
        {
            scale = value;
            GenerateMap();
        }
    }
    
    [ShowInInspector]
    [PropertySpace(4)]
    [PropertyRange(0, 10)]
    public int Octaves
    {
        get => octaves;
        set
        {
            octaves = value;
            GenerateMap();
        }
    }

    [ShowInInspector]
    [PropertySpace(4)]
    [PropertyRange(0, 10)]
    public float Lacunarity
    {
        get => lacunarity;
        set
        {
            lacunarity = value;
            GenerateMap();
        }
    }
    
    [ShowInInspector]
    [PropertySpace(4)]
    [PropertyRange(0, 1)]
    public float Persistance
    {
        get => persistance;
        set
        {
            persistance = value;
            GenerateMap();
        }
    }
    
    [ShowInInspector]
    [PropertySpace(4)]
    public Vector2 Offset
    {
        get => offset;
        set
        {
            offset = value;
            GenerateMap();
        }
    }
    
    private int octaves = 4;
    private float scale = 25f;
    private float lacunarity  = 2;
    private float persistance = 0.5f;
    private Vector2 offset;

    public override void OnStart(GridEngine engine)
    {
        this.engine = engine;
        
        GenerateMap();
    }

    private void GenerateMap()
    {
        if (engine == null)
            return;
        
        var basegroundGrid = engine.BasegroundGrid;
        var backgroundGrid = engine.BackgroundGrid;
        var foregroundGrid = engine.ForegroundGrid;
        
        NoiseMap noiseMap = new NoiseMap(basegroundGrid.Columns, 
                                         basegroundGrid.Rows, 
                                         0, scale, octaves, persistance, lacunarity, offset);
        
        basegroundGrid.ForEachTile(tile => PaintTile(tile, noiseMap, basegroundMap));

        backgroundGrid.ForEachTile(tile => PaintTile(tile, noiseMap, backgroundMap));
        foregroundGrid.ForEachTile(tile => PaintTile(tile, noiseMap, foregroundMap));

        backgroundGrid.ForEachTile(tile => tile.ApplyRule());
        foregroundGrid.ForEachTile(tile => tile.ApplyRule());
    }

    private void PaintTile(TerrainTile tile, NoiseMap noiseMap, MapSetting setting)
    {
        float perlinValue = noiseMap.Evaluate(tile.Coordinate);
        
        tile.Paint(setting.Evaluate(perlinValue));
    }
}