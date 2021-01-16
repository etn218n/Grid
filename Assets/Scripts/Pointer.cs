using Optional;
using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using GridSystem;

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
        var initialPosition = engine.MovementGrid.GetTileAt(Vector2Int.zero).Map(t => t.Position);
        
        initialPosition.MatchSome(point =>
        {
            selectedCharacters.ForEach(character =>
            {
                var zPostion = character.transform.position.z;
                character.transform.position = new Vector3(point.x, point.y, zPostion);
            });
        });

        selectedCharacters.ForEach(character => TryMoveRandomly(character, engine.MovementGrid));
    }

    private void TryMoveRandomly<T>(Character character, Grid<T> grid) where T : BaseTile<T>, IHaveMovementCost
    {
        var (source, destination) = RandomWaypointFor(grid, character);
            
        source.MatchSome(s => destination.MatchSome(d => character.Move(s, d)));
    }
    
    private void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            if (EventSystem.current.IsPointerOverGameObject())
                return;
            
            SetPathToCursorPosition();
        }
        
        selectedCharacters.ForEach(character =>
        {
            if (!character.Mover.IsMoving)
                TryMoveRandomly(character, engine.MovementGrid);
        });
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

    public (Option<T>, Option<T>) RandomWaypointFor<T>(Grid<T> grid, Character character) where T : BaseTile<T>, IHaveMovementCost
    {
        var source = grid.GetTileAt(character.transform.position);
        var destination = Option.None<T>();

        while (!destination.HasValue)
        {
            Vector2Int randomCoordinate = new Vector2Int(Random.Range(0, 50), Random.Range(0, 50));

            destination = grid.GetTileAt(randomCoordinate).Filter(tile => tile.IsWalkable);
        }

        return (source, destination);
    }

    private void DrawPath(Path path)
    {
        int i = 0;
        lineRenderer.positionCount = path.Count;
        
        path.ForEachPoint(p => lineRenderer.SetPosition(i++, p));
    }
}
