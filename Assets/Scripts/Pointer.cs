using UnityEngine;
using GridSystem;

public class Pointer : MonoBehaviour
{
    [SerializeField] private TerrainGridBehaviour backgroundGridBehaviour;
    [SerializeField] private TerrainGridBehaviour foregroundGridBehaviour;

    [SerializeField] private Terrain empty;
    
    private Grid<TerrainTile> backgroundGrid;
    private Grid<TerrainTile> foregroundGrid;

    private void Start()
    {
        backgroundGrid = backgroundGridBehaviour.Grid;
        foregroundGrid = foregroundGridBehaviour.Grid;
    }

    private void Update()
    {
        if (Input.GetMouseButton(0))
        {
            Vector3 mousePos = Input.mousePosition;
            mousePos.z = 10;
            Vector3 worldPosition = Camera.main.ScreenToWorldPoint(mousePos);

            var tile = foregroundGrid.TryGetTileAtPosition(worldPosition);

            if (tile != null)
            {
                tile.SetTerrain(empty);
                tile.Paint();
                tile.ForEachNeighbor(n => n.ApplyRule());
            }
        }
    }
}
