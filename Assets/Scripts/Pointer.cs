using UnityEngine;

public class Pointer : MonoBehaviour
{
    [SerializeField] private GridEngine engine;

    [SerializeField] private Terrain empty;

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mousePos = Input.mousePosition;
            mousePos.z = 10;
            Vector3 worldPosition = Camera.main.ScreenToWorldPoint(mousePos);

            var tile = engine.PlantGrid.TryGetTileAtPosition(worldPosition);

            if (tile != null)
            {
                tile.ShowPlantInfo();
                // tile.SetTerrain(empty);
                // tile.Paint();
                // tile.ForEachNeighbor(n => n.ApplyRule());
            }
        }
    }
}
