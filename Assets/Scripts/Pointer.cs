using UnityEngine;

public class Pointer : MonoBehaviour
{
    [SerializeField] private GridEngine engine;

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mouseScreenPosition = Input.mousePosition;
            mouseScreenPosition.z = 10;
            
            Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(mouseScreenPosition);

            var chunkA = engine.PlantGrid.TryGetChunkAt(mouseWorldPosition);
            var chunkB = engine.ForegroundGrid.TryGetChunkAt(mouseWorldPosition);

            if (chunkA != null)
            {
                chunkA.ToggleActive();
                chunkA.ToggleVisible();
                
                chunkB.ToggleActive();
                chunkB.ToggleVisible();
            }
        }
    }
}
