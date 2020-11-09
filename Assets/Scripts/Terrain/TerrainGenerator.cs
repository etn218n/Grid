using System;
using UnityEngine;
using GridSystem;
using Sirenix.OdinInspector;
using Random = UnityEngine.Random;

[RequireComponent(typeof(TerrainGrid))]
public class TerrainGenerator : MonoBehaviour
{
    private Grid<TerrainTile> grid = null;
    private int iteration = 1;
    private float scale = 1;
    private float offsetX = 0;
    private float offsetY = 0;
    private float grassCoverage = 0;
    private float soilCoverage = 0;

    //[Title("Game of Life")]
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
    [SerializeField] private Sprite sand  = null;
    [SerializeField] private Sprite soil  = null;
    [SerializeField] private Sprite grass = null;
    [SerializeField] private Sprite water = null;
    
    private Vector2[] sandUVs;
    private Vector2[] soilUVs;
    private Vector2[] grassUVs;
    private Vector2[] waterUVs;

    private void Start()
    {
        grid = GetComponent<TerrainGrid>().Grid;
        sandUVs = Extension.GetUVs(sand);
        soilUVs = Extension.GetUVs(soil);
        grassUVs = Extension.GetUVs(grass);
        waterUVs = Extension.GetUVs(water);
        GenerateMap();
    }

    private void RecalculateCoverage()
    {
        soilCoverage = 100 - grassCoverage;
        GenerateMap();
    }

    private void OnIterationChanceChanged()
    {
    }

    private void GenerateMap()
    {
        grid.ForEach(PaintTile);
    }

    private void PaintTile(Tile<TerrainTile> tile)
    {
        if (tile.IsEdge || tile.IsCorner)
        {
            tile.SetUVs(ref soilUVs);
        }
        else
        {
            float perlinValue = Mathf.PerlinNoise((tile.Coordinate.x / grid.Width) * scale + offsetX,
                                                  (tile.Coordinate.y / grid.Height) * scale + offsetY);

            if (perlinValue > 0.5f)
                tile.SetUVs(ref soilUVs);
            else if (perlinValue > 0.25f)
                tile.SetUVs(ref grassUVs);
            else if (perlinValue > 0.22f)
                tile.SetUVs(ref sandUVs);
            else
                tile.SetUVs(ref waterUVs);
        }
    }
}