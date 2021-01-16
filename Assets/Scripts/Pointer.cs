using Optional;
using UnityEngine;
using UnityEngine.EventSystems;
using System.Linq;
using System.Collections.Generic;
using GridSystem;
using PriorityQueue;

public class Pointer : MonoBehaviour
{
    [SerializeField] private GridEngine engine;

    private LineRenderer lineRenderer;

    public List<Character> selectedCharacters = new List<Character>();

    private void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
    }

    private void Start()
    {
        var initialTile = engine.MovementGrid.GetTileAt(new Vector2Int(0, 0)).Map(t => t.Position);
        
        initialTile.MatchSome(point =>
        {
            var zPostion = selectedCharacters[0].transform.position.z;
            selectedCharacters[0].transform.position = new Vector3(point.x, point.y, zPostion);
        });
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (EventSystem.current.IsPointerOverGameObject())
                return;
            
            SetPathToCursorPosition();
        }
    }

    public void SetPathToCursorPosition()
    {
        Vector3 mouseScreenPosition = Input.mousePosition;
        mouseScreenPosition.z = 10;
            
        Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(mouseScreenPosition);

        var character = selectedCharacters[0];
        
        var mouseTile = engine.MovementGrid.GetTileAt(mouseWorldPosition);
        var characterTile = engine.MovementGrid.GetTileAt(character.transform.position);
        
        mouseTile.MatchSome(mTile =>
        {
            characterTile.MatchSome(cTile =>
            { 
                character.Move(cTile, mTile);
            });
        });
    }

    private void DrawPath(Path path)
    {
        int i = 0;
        lineRenderer.positionCount = path.Count;
        
        path.ForEachPoint(p => lineRenderer.SetPosition(i++, p));
    }
}
