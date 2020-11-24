using System;
using UnityEngine;
using GridSystem;

public class GridBehaviour<T> : MonoBehaviour where T : BaseTile<T>
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

    [Header("External Components")]
    [SerializeField] protected GameObject gridMeshPrefab = null;

    protected void Create(Func<Grid<T>, Chunk, Vector2Int, Vector2Int, T> instantiationFunc)
    {
        float startTime = Time.realtimeSinceStartup;

        grid = new Grid<T>(this.transform.position, horizontalChunks, verticalChunks, rowsPerChunk, columnsPerChunk, tileSize, instantiationFunc);
        Debug.Log( $"{ToString()} Creation Time: " + (Time.realtimeSinceStartup - startTime).ToString("F5") + "s");

        foreach (var chunk in grid.Chunks)
        {
            GameObject go = Instantiate(gridMeshPrefab, this.transform.position, Quaternion.identity, this.transform);

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
