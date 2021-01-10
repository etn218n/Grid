using UnityEngine;
using System.Collections.Generic;
using GridSystem;

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
        
        modules.ForEach(module => module.OnStart(this));
    }

    private void InstantiateGrids()
    {
        plantGrid = new Grid<PlantTile>(plantOrigin, horizontalChunks, verticalChunks, rowsPerChunk, columnsPerChunk, tileSize,
                                        (ownerGrid, coordinate) => new PlantTile(ownerGrid, coordinate));

        movementGrid = new Grid<MovementTile>(movementOrigin, horizontalChunks, verticalChunks, rowsPerChunk, columnsPerChunk, tileSize, 
                                              (ownerGrid, coordinate) => new MovementTile(ownerGrid, coordinate));
        
        basegroundGrid = new Grid<TerrainTile>(basegroundOrigin, horizontalChunks, verticalChunks, rowsPerChunk, columnsPerChunk, tileSize, 
                                               (ownerGrid, coordinate) => new TerrainTile(ownerGrid, coordinate));

        backgroundGrid = new Grid<TerrainTile>(backgroundOrigin, horizontalChunks, verticalChunks, rowsPerChunk, columnsPerChunk, tileSize, 
                                               (ownerGrid, coordinate) => new TerrainTile(ownerGrid, coordinate));

        foregroundGrid = new Grid<TerrainTile>(foregroundOrigin, horizontalChunks, verticalChunks, rowsPerChunk, columnsPerChunk, tileSize, 
                                               (ownerGrid, coordinate) => new TerrainTile(ownerGrid, coordinate));
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
        
        UpdateGridsMesh();
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

    private void UpdateGridsMesh()
    {
        plantGrid.UpdateMesh();
        movementGrid.UpdateMesh();
        basegroundGrid.UpdateMesh();
        backgroundGrid.UpdateMesh();
        foregroundGrid.UpdateMesh();
    }
    
    private void DrawGrids()
    {
        plantGrid.Draw(plantMaterial);
        movementGrid.Draw(movementMaterial);
        basegroundGrid.Draw(basegroundMaterial);
        backgroundGrid.Draw(backgroundMaterial);
        foregroundGrid.Draw(foregroundMaterial);
    }

    private void OnDestroy()
    {
        plantGrid.Dispose();
        movementGrid.Dispose();
        basegroundGrid.Dispose();
        backgroundGrid.Dispose();
        foregroundGrid.Dispose();
        
        modules.ForEach(module => module.OnEnd(this));
    }
}
