using UnityEngine;
using GridSystem;

public class DummyGrid : MonoBehaviour
{
    private Grid<int> grid = null;

    [Header("Grid Dimension")]
    [SerializeField] private int horizontalChunks;
    [SerializeField] private int verticalChunks;
    
    [Header("Chunk Dimension")]
    [SerializeField] private int rowsPerChunk;
    [SerializeField] private int columnsPerChunk;
    
    [Header("Other Settings")]
    [SerializeField] private float tileSize;
    [SerializeField] private Vector3 origin = Vector3.zero;

    [Header("External Components")]
    [SerializeField] GameObject gridMeshPrefab = null;
    [SerializeField] private Sprite ground = null;
    [SerializeField] private Sprite plant  = null;
    
    private void Awake()
    {
        float startTime = Time.realtimeSinceStartup;
        grid = new Grid<int>(origin, horizontalChunks, verticalChunks, rowsPerChunk, columnsPerChunk, tileSize, (tile) => 1);
        Debug.Log( "Grid Creation Time: " + (Time.realtimeSinceStartup - startTime).ToString("F5") + "s");
        
        Vector2[] groundUVs = Extension.GetUVs(ground);
        
        foreach (var chunk in grid.Chunks)
        {
            var go = GameObject.Instantiate(gridMeshPrefab, Vector3.zero, Quaternion.identity, this.transform);

            go.GetComponent<MeshFilter>().mesh = chunk.Mesh;
            chunk.SetChunkUVs(ref groundUVs);
        }
    }

    private void Update()
    {
        if (Input.GetMouseButton(0))
        {
            Vector3 mousePos = Input.mousePosition;
            mousePos.z = 10;
            Vector3 worldPosition = Camera.main.ScreenToWorldPoint(mousePos);

            var tile = grid.TryGetTileAtPosition(worldPosition);

            if (tile != null)
            {
                Vector2[] uvs = Extension.GetUVs(plant);
                tile.SetUVs(ref uvs);
                
                if (tile.EastNeighbor != null)
                    tile.EastNeighbor.SetUVs(ref uvs);
                
                if (tile.WestNeighbor != null)
                    tile.WestNeighbor.SetUVs(ref uvs);
                
                if (tile.SouthNeighbor != null)
                    tile.SouthNeighbor.SetUVs(ref uvs);
                
                if (tile.NorthNeighbor != null)
                    tile.NorthNeighbor.SetUVs(ref uvs);
            }
        }
        
        grid.Update();
    }

    private void OnDestroy()
    {
        grid.Dispose();
    }
}
