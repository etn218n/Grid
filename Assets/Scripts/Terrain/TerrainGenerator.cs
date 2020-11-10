using System;
using UnityEngine;
using GridSystem;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using Random = UnityEngine.Random;

public class TerrainGenerator : MonoBehaviour
{
    [SerializeField] private TerrainGridBehaviour baseGridBehaviour = null;
    [SerializeField] private TerrainGridBehaviour topGridBehaviour  = null;
    private Grid<TerrainTile> baseGrid;
    private Grid<TerrainTile> topGrid;
    private int iteration = 1;
    private float scale = 1;
    private float offsetX = 0;
    private float offsetY = 0;
    private float grassCoverage = 0;
    private float soilCoverage = 0;

    [SerializeField] private Sprite ruleTile = null;

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
    [SerializeField] private Sprite empty = null;

    private Vector2[] sandUVs;
    private Vector2[] soilUVs;
    private Vector2[] grassUVs;
    private Vector2[] waterUVs;
    private Vector2[] ruleTileUVs;
    private Vector2[] emptyUVs;

    private void Start()
    {
        baseGrid = baseGridBehaviour.Grid;
        topGrid  = topGridBehaviour.Grid;
        sandUVs  = Extension.GetUVs(sand);
        soilUVs  = Extension.GetUVs(soil);
        grassUVs = Extension.GetUVs(grass);
        waterUVs = Extension.GetUVs(water);
        emptyUVs = Extension.GetUVs(empty);
        ruleTileUVs = Extension.GetUVs(ruleTile);
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
        baseGrid.ForEach(PaintTile);
        topGrid.ForEach(PainEmptyTile);
        topGrid.ForEach(ApplyTileRule);
    }

    private void PainEmptyTile(Tile<TerrainTile> tile)
    {
        tile.SetUVs(ref emptyUVs);
    }

    private void PaintTile(Tile<TerrainTile> tile)
    {
        if (tile.IsEdge || tile.IsCorner)
        {
            tile.SetUVs(ref soilUVs);
            tile.Element.Type = TerrainType.Soil;
        }
        else
        {
            float perlinValue = Mathf.PerlinNoise((tile.Coordinate.x / baseGrid.Width) * scale + offsetX,
                                                  (tile.Coordinate.y / baseGrid.Height) * scale + offsetY);

            if (perlinValue > 0.5f)
            {
                tile.SetUVs(ref soilUVs);
                tile.Element.Type = TerrainType.Soil;
            }
            else if (perlinValue > 0.25f)
            {
                tile.SetUVs(ref grassUVs);
                tile.Element.Type = TerrainType.Grass;
            }
            else if (perlinValue > 0.22f)
            {
                tile.SetUVs(ref sandUVs);
                tile.Element.Type = TerrainType.Sand;
            }
            else
            {
                tile.SetUVs(ref waterUVs);
                tile.Element.Type = TerrainType.Water;
            }
        }
    }

    private void ApplyTileRule(Tile<TerrainTile> tile)
    {
        var baseTile = baseGrid.TryGetTileAtCoordinate(tile.Coordinate);
        var baseTileElement = baseTile.Element;

        if (baseTileElement.Type == TerrainType.Soil && baseTile.EastNeighbor != null && baseTile.EastNeighbor.Element.Type == TerrainType.Grass)
        {
            tile.SetUVs(ref ruleTileUVs);
            baseTile.SetUVs(ref grassUVs);
        }

        if (baseTile.IsCorner)
        {
            Debug.Log(baseTileElement.Type);
        }
             
    }
}