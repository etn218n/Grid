using System;
using GridSystem;
using UnityEngine;
using Grid = GridSystem.Grid;
using Random = System.Random;

public class Dummy : MonoBehaviour
{
    [Header("Grid Dimension")]
    [SerializeField] protected int horizontalChunks;
    [SerializeField] protected int verticalChunks;

    [Header("Chunk Dimension")]
    [SerializeField] protected int rowsPerChunk;
    [SerializeField] protected int columnsPerChunk;

    [Header("Other Settings")]
    [SerializeField] protected float tileSize = 1;

    [Header("External Components")] 
    [SerializeField] protected GameObject gridMeshPrefab;

    private Grid grid;
    public  Grid Grid => grid;
    
    private void Awake()
    {
        grid = new Grid(this.transform.position, horizontalChunks, verticalChunks, rowsPerChunk, columnsPerChunk, tileSize);

        foreach (var chunk in grid.Chunks)
        {
            GameObject go = Instantiate(gridMeshPrefab, this.transform.position, Quaternion.identity, this.transform);

            go.GetComponent<MeshFilter>().mesh = chunk.Mesh;
        }

        grid.SetUV(in Rect2D.Empty);
        
        grid.ForEachCoordinate(coordinate =>
        {
            Rect3D verticesRect = grid.GetTileVertices(coordinate);

            Vector3 bottomLeft  = verticesRect.BottomLeft;
            Vector3 topLeft     = verticesRect.TopLeft;
            Vector3 bottomRight = verticesRect.BottomRight;
            Vector3 topRight    = verticesRect.TopRight;
            
            topLeft.y  += grid.TileSize;
            topRight.y += grid.TileSize;

            float zValue = (bottomLeft.y) * 0.0001f;
            bottomLeft.z  = zValue;
            topLeft.z     = zValue;
            bottomRight.z = zValue;
            topRight.z    = zValue;
 
            grid.SetTileVertices(new Rect3D(bottomLeft, topLeft, bottomRight, topRight), coordinate);
        });
    }
    
    protected void Update()
    {
        if (grid != null)
            grid.Update();
    }

    protected void OnDestroy()
    {
        if (grid != null)
            grid.Dispose();
    }
}