using UnityEngine;
using GridSystem;
using Sirenix.OdinInspector;

public class TerrainGenerator : MonoBehaviour
{
    [SerializeField] private TerrainGridBehaviour basegroundGridBehaviour = null;
    [SerializeField] private TerrainGridBehaviour backgroundGridBehaviour = null;
    [SerializeField] private TerrainGridBehaviour foregroundGridBehaviour = null;

    private Grid<TerrainTile> baseGroundGrid;
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
    [SerializeField] private Terrain empty;
    [SerializeField] private Terrain grass;
    [SerializeField] private Terrain hill;
    [SerializeField] private Terrain sand;
    [SerializeField] private Terrain water;
    [SerializeField] private Terrain whiteSoil;

    private void Start()
    {
        baseGroundGrid = basegroundGridBehaviour.Grid;
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
        baseGroundGrid.ForEach(tile => tile.Paint(grass));
        
        backgroundGrid.ForEach(tile => ApplyBackground(tile));
        backgroundGrid.ForEach(tile => tile.ApplyRule());
        
        foreGroundGrid.ForEach(tile => PaintTile(tile));
        foreGroundGrid.ForEach(tile => tile.ApplyRule());
    }
    
    

    private void ApplyBackground(TerrainTile tile)
    {
        if (tile.IsEdge || tile.IsCorner)
        {
            tile.Paint(whiteSoil);
        }
        else
        {
            float perlinValue = Mathf.PerlinNoise((tile.Coordinate.x / backgroundGrid.Width)  * scale + offsetX,
                                                  (tile.Coordinate.y / backgroundGrid.Height) * scale + offsetY);
        
            if (perlinValue > 0.5f)
            {
                tile.Paint(whiteSoil);
            }
            else if (perlinValue > 0.25f)
            {
                tile.Paint(empty);
            }
            else 
            {
                tile.Paint(sand);
            }
        }
    }
    
    private void PaintTile(TerrainTile tile)
    {
        if (tile.IsEdge || tile.IsCorner)
        {
            tile.Paint(hill);
        }
        else
        {
            float perlinValue = Mathf.PerlinNoise((tile.Coordinate.x / backgroundGrid.Width)  * scale + offsetX,
                                                  (tile.Coordinate.y / backgroundGrid.Height) * scale + offsetY);
        
            if (perlinValue > 0.5f)
            {
                tile.Paint(hill);
            }
            else if (perlinValue > 0.25f)
            {
                tile.Paint(grass);
            }
            else if (perlinValue > 0.22f)
            {
                tile.Paint(sand);
            }
            else
            {
                tile.Paint(water);
            }
        }
    }
}