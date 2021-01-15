using Optional;
using UnityEngine;
using UnityEngine.EventSystems;
using System.Linq;
using System.Collections.Generic;

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
                CalculateBreadthFirstPath(cTile, mTile, character.transform.position.z).MatchSome(path =>
                {
                    DrawPath(path);
                    character.Move(path);
                });
            });
        });
    }

    private void DrawPath(Path path)
    {
        int i = 0;
        lineRenderer.positionCount = path.Count;
        
        path.ForEachPoint(p => lineRenderer.SetPosition(i++, p));
    }

    private Queue<Vector3> CalculateManhattanPath(MovementTile source, MovementTile destination, float z, float stepSize)
    {
        var path = new Queue<Vector3>();

        var horizontalSteps = (int)(Mathf.Abs(destination.Position.x - source.Position.x) / stepSize);
        var verticalSteps   = (int)(Mathf.Abs(destination.Position.y - source.Position.y) / stepSize);

        var deltaX = (destination.Position.x - source.Position.x);
        var deltaY = (destination.Position.y - source.Position.y);

        var horizontalDestination = Option.Some(source);
        
        if (deltaX > 0)
        {
            horizontalDestination = source.TraverseEast(horizontalSteps, 
                                                        tile => path.Enqueue(new Vector3(tile.Position.x, tile.Position.y, z)));
        }
        else if (deltaX < 0)
        {
            horizontalDestination = source.TraverseWest(horizontalSteps, 
                                                        tile => path.Enqueue(new Vector3(tile.Position.x, tile.Position.y, z)));
        }

        if (deltaY > 0)
        {
            horizontalDestination.MatchSome(t => t.TraverseNorth(verticalSteps, 
                                                                 tile => path.Enqueue(new Vector3(tile.Position.x, tile.Position.y, z))));
        }
        else if (deltaY < 0)
        {
            horizontalDestination.MatchSome(t => t.TraverseSouth(verticalSteps, 
                                                                 tile => path.Enqueue(new Vector3(tile.Position.x, tile.Position.y, z))));
        }

        return path;
    }

    private Option<Path> CalculateBreadthFirstPath(MovementTile source, MovementTile destination, float z)
    {
        var frontier = new Queue<MovementTile>();
        var cameFrom = new Dictionary<MovementTile, MovementTile>();
        
        frontier.Enqueue(source);

        while (frontier.Any())
        {
            var current = frontier.Dequeue();
            
            if (current == destination)
                break;
            
            current.ForEachCardinalNeighbor(next =>
            {
                if (!cameFrom.ContainsKey(next) && next.MovementCost != 10)
                {
                    frontier.Enqueue(next);
                    cameFrom.Add(next, current);
                }
            });
        }

        return ConstructPath(cameFrom, source, destination, z);
    }

    private Option<Path> ConstructPath(Dictionary<MovementTile, MovementTile> cameFrom, MovementTile source, MovementTile destination, float z)
    {
        if (!cameFrom.ContainsKey(destination))
            return Option.None<Path>();

        var path = new Path();
        
        var currentTile = destination;
        
        while (currentTile != source)
        {
            path.AddPoint(currentTile.Position.x, currentTile.Position.y, z);
            currentTile = cameFrom[currentTile];
        }
        
        path.Reverse();
        
        return Option.Some(path);
    }
}
