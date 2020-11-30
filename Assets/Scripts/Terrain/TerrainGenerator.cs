using UnityEngine;
using GridSystem;
using Sirenix.OdinInspector;

public class TerrainGenerator : MonoBehaviour
{
    [Header("Grids")] 
    [SerializeField] private TerrainGridBehaviour basegroundGridBehaviour = null;
    [SerializeField] private TerrainGridBehaviour backgroundGridBehaviour = null;
    [SerializeField] private TerrainGridBehaviour foregroundGridBehaviour = null;

    private Grid<TerrainTile> baseGroundGrid;
    private Grid<TerrainTile> backgroundGrid;
    private Grid<TerrainTile> foreGroundGrid;
    
    [Header("Terrains")] 
    [SerializeField] private Terrain empty;
    [SerializeField] private Terrain grass;
    [SerializeField] private Terrain hill;
    [SerializeField] private Terrain sand;
    [SerializeField] private Terrain water;
    [SerializeField] private Terrain whiteSoil;

    private int octaves = 1;
    private float scale = 25f;
    private float lacunarity  = 1;
    private float persistance = 0.5f;
    private Vector2 offset;

    private NoiseMap noiseMap;

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

    private void GenerateMap()
    {
        noiseMap = new NoiseMap(baseGroundGrid.Columns, 
                                baseGroundGrid.Rows, 
                                0, scale, octaves, persistance, lacunarity, offset);
        
        baseGroundGrid.ForEach(tile => tile.Paint(grass));

        backgroundGrid.ForEach(tile => ApplyBackground(tile));
        foreGroundGrid.ForEach(tile => PaintTile(tile));

        backgroundGrid.ForEach(tile => tile.ApplyRule());
        foreGroundGrid.ForEach(tile => tile.ApplyRule());
    }
    
    private void ApplyBackground(TerrainTile tile)
    {
        float perlinValue = noiseMap.Evaluate(tile.Coordinate);
        
        if (perlinValue.IsBetween(0.7f, 1.0f))
            tile.Paint(whiteSoil);
        else if (perlinValue.IsBetween(0.4f, 0.7f))
            tile.Paint(empty);
        else
            tile.Paint(sand);
    }
    
    private void PaintTile(TerrainTile tile)
    {
        float perlinValue = noiseMap.Evaluate(tile.Coordinate);
        
        if (perlinValue.IsBetween(0.7f, 1.0f))
            tile.Paint(hill);
        else if (perlinValue.IsBetween(0.4f, 0.7f))
            tile.Paint(grass);
        else if (perlinValue.IsBetween(0.35f, 0.4f))
            tile.Paint(sand);
        else
            tile.Paint(water);
    }
}