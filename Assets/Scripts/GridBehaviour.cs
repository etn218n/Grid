using System;
using System.Collections.Generic;
using UnityEngine;
using GridSystem;

public class GridBehaviour<T> : MonoBehaviour
{ 
    protected Grid<T> grid;
    public Grid<T> Grid => grid;
    
    [Header("Grid Dimension")]
    [SerializeField] protected int horizontalChunks;
    [SerializeField] protected int verticalChunks;

    [Header("Chunk Dimension")]
    [SerializeField] protected int rowsPerChunk;
    [SerializeField] protected int columnsPerChunk;

    [Header("Other Settings")]
    [SerializeField] protected float tileSize = 1;
    [SerializeField] protected Vector3 origin = Vector3.zero;

    [Header("External Components")]
    [SerializeField] protected GameObject gridMeshPrefab = null;

    protected void Create(Func<Tile<T>, T> func)
    {
        float startTime = Time.realtimeSinceStartup;

        grid = new Grid<T>(origin, horizontalChunks, verticalChunks, rowsPerChunk, columnsPerChunk, tileSize, func);
        Debug.Log( $"{ToString()} Creation Time: " + (Time.realtimeSinceStartup - startTime).ToString("F5") + "s");

        foreach (var chunk in grid.Chunks)
        {
            GameObject go = Instantiate(gridMeshPrefab, Vector3.zero, Quaternion.identity, this.transform);

            go.GetComponent<MeshFilter>().mesh = chunk.Mesh;
        }
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
