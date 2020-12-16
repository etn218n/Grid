using UnityEngine;

public class Pointer : MonoBehaviour
{
    [SerializeField] private GridEngine engine;

    private void Update()
    {
        if (Input.GetMouseButton(0))
        {
            Vector3 mouseScreenPosition = Input.mousePosition;
            mouseScreenPosition.z = 10;
            
            Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(mouseScreenPosition);

            var tile = engine.PlantGrid.TryGetTileAt(mouseWorldPosition);

            if (tile != null)
                tile.RemovePlant();
        }
    }
}
