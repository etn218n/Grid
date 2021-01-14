using UnityEngine;
using System.Collections.Generic;
using GridSystem;
using Grid = GridSystem.Grid;

public class GridEngine : MonoBehaviour
{
    [Header("Update Setting")] 
    [Range(1, 3)]
    [SerializeField] private int gameSpeed = 1;
    [SerializeField] private int ticksPerSecond = 60;
    
    [Header("Grid Dimension")]
    [SerializeField] private int horizontalChunks;
    [SerializeField] private int verticalChunks;
    [SerializeField] private float tileSize = 1;

    [Header("Chunk Dimension")]
    [SerializeField] [Range(1, 100)] private int rowsPerChunk;
    [SerializeField] [Range(1, 100)] private int columnsPerChunk;

    [Header("Grid Origin")] 
    [SerializeField] private Vector3 basegroundOrigin;
    [SerializeField] private Vector3 backgroundOrigin;
    [SerializeField] private Vector3 foregroundOrigin;
    [SerializeField] private Vector3 movementOrigin;
    [SerializeField] private Vector3 plantOrigin;

    [Header("Draw Settings")]
    [SerializeField] private Material basegroundMaterial;
    [SerializeField] private Material backgroundMaterial;
    [SerializeField] private Material foregroundMaterial;
    [SerializeField] private Material movementMaterial;
    [SerializeField] private Material plantMaterial;

    [Header("Modules")] 
    [SerializeField] private List<GridEngineModule> modules = new List<GridEngineModule>();

    private HashSet<Grid> visibleGrids = new HashSet<Grid>();
    
    private Grid<TerrainTile> basegroundGrid;
    private Grid<TerrainTile> backgroundGrid;
    private Grid<TerrainTile> foregroundGrid;
    private Grid<MovementTile> movementGrid;
    private Grid<PlantTile> plantGrid;

    private float frameTime;
    private float elapsedTime;
    private long ticksSinceStartUp;
    private bool alreadyTickedThisFrameTime;
    
    public Grid<TerrainTile> BasegroundGrid => basegroundGrid;
    public Grid<TerrainTile> BackgroundGrid => backgroundGrid;
    public Grid<TerrainTile> ForegroundGrid => foregroundGrid;
    public Grid<MovementTile> MovementGrid => movementGrid;
    public Grid<PlantTile> PlantGrid => plantGrid;

    private void Awake()
    {
        InstantiateGrids();
        AddVisibleGrids();
        
        modules.ForEach(module => module.OnStart(this));
    }

    private void InstantiateGrids()
    {
        plantGrid = new Grid<PlantTile>(plantOrigin, horizontalChunks, verticalChunks, rowsPerChunk, columnsPerChunk, tileSize, plantMaterial,
                                        (ownerGrid, coordinate) => new PlantTile(ownerGrid, coordinate));

        movementGrid = new Grid<MovementTile>(movementOrigin, horizontalChunks, verticalChunks, rowsPerChunk, columnsPerChunk, tileSize, movementMaterial, 
                                              (ownerGrid, coordinate) => new MovementTile(ownerGrid, coordinate));
        
        basegroundGrid = new Grid<TerrainTile>(basegroundOrigin, horizontalChunks, verticalChunks, rowsPerChunk, columnsPerChunk, tileSize, basegroundMaterial,
                                               (ownerGrid, coordinate) => new TerrainTile(ownerGrid, coordinate));

        backgroundGrid = new Grid<TerrainTile>(backgroundOrigin, horizontalChunks, verticalChunks, rowsPerChunk, columnsPerChunk, tileSize, backgroundMaterial,
                                               (ownerGrid, coordinate) => new TerrainTile(ownerGrid, coordinate));

        foregroundGrid = new Grid<TerrainTile>(foregroundOrigin, horizontalChunks, verticalChunks, rowsPerChunk, columnsPerChunk, tileSize, foregroundMaterial,
                                               (ownerGrid, coordinate) => new TerrainTile(ownerGrid, coordinate));
    }

    private void AddVisibleGrids()
    {
        visibleGrids.Add(plantGrid);
        visibleGrids.Add(movementGrid);
        visibleGrids.Add(basegroundGrid);
        visibleGrids.Add(backgroundGrid);
        visibleGrids.Add(foregroundGrid);
    }
    
    private void Update()
    {
        elapsedTime += Time.unscaledDeltaTime;
        
        frameTime = 1.0f / ticksPerSecond;

        if (elapsedTime < frameTime)
        {
            if (!alreadyTickedThisFrameTime)
            {
                for (int i = 0; i < gameSpeed; i++)
                    Tick();

                alreadyTickedThisFrameTime = true;
            }
        }
        else
        {
            alreadyTickedThisFrameTime = false;
            elapsedTime -= frameTime;
        }
        
        DrawGrids();
    }

    private void Tick()
    {
        plantGrid.Tick(ticksSinceStartUp);
        movementGrid.Tick(ticksSinceStartUp);
        basegroundGrid.Tick(ticksSinceStartUp);
        backgroundGrid.Tick(ticksSinceStartUp);
        foregroundGrid.Tick(ticksSinceStartUp);

        ticksSinceStartUp++;
    }
    
    private void DrawGrids()
    {
        foreach (var grid in visibleGrids)
        {
            grid.UpdateMesh();
            grid.Draw();
        }
    }

    private void OnDestroy()
    {
        modules.ForEach(module => module.OnEnd(this));
        
        plantGrid.Dispose();
        movementGrid.Dispose();
        basegroundGrid.Dispose();
        backgroundGrid.Dispose();
        foregroundGrid.Dispose();
    }

    public void ShowMovementGrid() => visibleGrids.Add(movementGrid);
    public void HideMovementGrid() => visibleGrids.Remove(movementGrid);
    public void ShowForegroundGrid() => visibleGrids.Add(foregroundGrid);
    public void HideForegroundGrid() => visibleGrids.Remove(foregroundGrid);

    public void ToggleMovementGridVisibility()
    {
        if (visibleGrids.Contains(movementGrid))
            HideMovementGrid();
        else 
            ShowMovementGrid();
    }
    
    public void ToggleForegroundGridVisibility()
    {
        if (visibleGrids.Contains(foregroundGrid))
            HideForegroundGrid();
        else
            ShowForegroundGrid();
    }
}
