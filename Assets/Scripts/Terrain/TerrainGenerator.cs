using UnityEngine;
using GridSystem;
using Sirenix.OdinInspector;

public class TerrainGenerator : MonoBehaviour
{
    [SerializeField] private TerrainGridBehaviour backgroundGridBehaviour = null;
    [SerializeField] private TerrainGridBehaviour foregroundGridBehaviour = null;
    
    private Grid<TerrainTile> backgroundGrid;
    private Grid<TerrainTile> foreGroundGrid;
    private int iteration = 1;
    private float scale = 1;
    private float offsetX = 0;
    private float offsetY = 0;
    private float grassCoverage = 0;
    private float soilCoverage = 0;
    
    [PropertySpace(4)]
    [PropertyRange(0, 100)]
    public float GrassCoverage
    {
        get => grassCoverage;
        set
        {
            grassCoverage = value;
            RecalculateCoverage();
        }
    }

    [PropertySpace(4)]
    [PropertyRange(0, 100)]
    public float SoilCoverage
    {
        get => soilCoverage;
        set
        {
            soilCoverage = value;
            RecalculateCoverage();
        }
    }

    [PropertyRange(0, 10)]
    public int Iteration
    {
        get => iteration;
        set
        {
            iteration = value;
            OnIterationChanceChanged();
        }
    }

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
    [PropertyRange(0, 100)]
    public float OffsetX
    {
        get => offsetX;
        set
        {
            offsetX = value;
            GenerateMap();
        }
    }

    [ShowInInspector]
    [PropertySpace(4)]
    [PropertyRange(0, 100)]
    public float OffsetY
    {
        get => offsetY;
        set
        {
            offsetY = value;
            GenerateMap();
        }
    }

    [Header("External Components")] 
    [SerializeField] private Terrain grass;
    [SerializeField] private Terrain hill;
    [SerializeField] private Terrain sand;
    [SerializeField] private Terrain water;

    private void Start()
    {
        backgroundGrid = backgroundGridBehaviour.Grid;
        foreGroundGrid = foregroundGridBehaviour.Grid;

        GenerateMap();
    }

    private void RecalculateCoverage()
    {
        GenerateMap();
    }

    private void OnIterationChanceChanged()
    {
    }

    private void GenerateMap()
    {
        backgroundGrid.ForEach(tile => PaintTile(tile));
        foreGroundGrid.ForEach(tile => PaintTile(tile));
        foreGroundGrid.ForEach(tile => tile.ApplyRule());
        backgroundGrid.ForEach(tile => BlendTile(tile));
    }

    private void BlendTile(TerrainTile tile)
    {
        if (tile.Terrain == hill)
        {
            tile.SetTerrain(grass);
            tile.Paint();
        }
    }
    
    private void PaintTile(TerrainTile tile)
    {
        if (tile.IsEdge || tile.IsCorner)
        {
            tile.SetTerrain(hill);
            tile.Paint();
        }
        else
        {
            float perlinValue = Mathf.PerlinNoise((tile.Coordinate.x / backgroundGrid.Width)  * scale + offsetX,
                                                  (tile.Coordinate.y / backgroundGrid.Height) * scale + offsetY);
        
            if (perlinValue > 0.5f)
            {
                tile.SetTerrain(hill);
                tile.Paint();
            }
            else if (perlinValue > 0.25f)
            {
                tile.SetTerrain(grass);
                tile.Paint();
            }
            else if (perlinValue > 0.22f)
            {
                tile.SetTerrain(sand);
                tile.Paint();
            }
            else
            {
                tile.SetTerrain(water);
                tile.Paint();
            }
        }
    }
}