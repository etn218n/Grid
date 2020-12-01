using UnityEngine;
using GridSystem;
using Sirenix.OdinInspector;

public class TerrainGenerator : MonoBehaviour
{
    [Header("Grids")] 
    [SerializeField] private TerrainGridBehaviour basegroundGridBehaviour;
    [SerializeField] private TerrainGridBehaviour backgroundGridBehaviour;
    [SerializeField] private TerrainGridBehaviour foregroundGridBehaviour;

    private Grid<TerrainTile> baseGroundGrid;
    private Grid<TerrainTile> backgroundGrid;
    private Grid<TerrainTile> foreGroundGrid;

    [Header("Terrains")] 
    [SerializeField] private MapSetting backgroundMap;
    [SerializeField] private MapSetting foregroundMap;
    
    [Header("Terrains")] 
    [SerializeField] private Terrain empty;
    [SerializeField] private Terrain grass;
    [SerializeField] private Terrain hill;
    [SerializeField] private Terrain sand;
    [SerializeField] private Terrain water;
    [SerializeField] private Terrain whiteSoil;

    private int octaves = 4;
    private float scale = 25f;
    private float lacunarity  = 2;
    private float persistance = 0.5f;
    private Vector2 offset;

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
    
    private void Start()
    {
        baseGroundGrid = basegroundGridBehaviour.Grid;
        backgroundGrid = backgroundGridBehaviour.Grid;
        foreGroundGrid = foregroundGridBehaviour.Grid;

        GenerateMap();
    }
    
    [Button]
    private void GenerateMap()
    {
        NoiseMap noiseMap = new NoiseMap(baseGroundGrid.Columns, 
                                         baseGroundGrid.Rows, 
                                         0, scale, octaves, persistance, lacunarity, offset);
        
        baseGroundGrid.ForEach(tile => tile.Paint(grass));

        backgroundGrid.ForEach(tile => PaintTile(tile, noiseMap, backgroundMap));
        foreGroundGrid.ForEach(tile => PaintTile(tile, noiseMap, foregroundMap));

        backgroundGrid.ForEach(tile => tile.ApplyRule());
        foreGroundGrid.ForEach(tile => tile.ApplyRule());
    }

    private void PaintTile(TerrainTile tile, NoiseMap noiseMap, MapSetting setting)
    {
        float perlinValue = noiseMap.Evaluate(tile.Coordinate);
        
        tile.Paint(setting.Evaluate(perlinValue));
    }
}