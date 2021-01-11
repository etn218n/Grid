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

            var optionalChunkA = engine.PlantGrid.GetChunkAt(mouseWorldPosition);
            
            optionalChunkA.MatchSome(chunk =>
            {
                chunk.ToggleActive();
                chunk.ToggleVisible();
            });
            
            var optionalChunkB = engine.ForegroundGrid.GetChunkAt(mouseWorldPosition);
            
            optionalChunkB.MatchSome(chunk =>
            {
                chunk.ToggleActive();
                chunk.ToggleVisible();
            });
        }
    }
}
