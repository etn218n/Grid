using Optional;
using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using System.Linq;

public class Pointer : MonoBehaviour
{
    [SerializeField] private GridEngine engine;

    public List<Character> selectedCharacters = new List<Character>();
    
    private void Start()
    {
        var o = engine.MovementGrid.GetTileAt(new Vector2Int(0, 0)).Map(t => t.Position);
        
        o.MatchSome(point =>
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
            
            Vector3 mouseScreenPosition = Input.mousePosition;
            mouseScreenPosition.z = 10;
            
            Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(mouseScreenPosition);

            engine.MovementGrid.GetTileAt(mouseWorldPosition).MatchSome(destination =>
            {
                selectedCharacters.ForEach(character =>
                {
                    engine.MovementGrid.GetTileAt(character.transform.position).MatchSome(source =>
                    {
                        //var path = CalculateManhattanPath(source, destination, character.transform.position.z, engine.MovementGrid.TileSize);
                        var path = CalculateBreadthFirstPath(source, destination, character.transform.position.z);
                        character.Move(path);
                    });
                });
            });
        }
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

    private Queue<Vector3> CalculateBreadthFirstPath(MovementTile source, MovementTile destination, float z)
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
                if (!cameFrom.ContainsKey(next))
                {
                    frontier.Enqueue(next);
                    cameFrom.Add(next, current);
                }
            });
        }

        var path = new Queue<Vector3>();
        
        var currentTile = destination;
        
        while (currentTile != source)
        {
            path.Enqueue(new Vector3(currentTile.Position.x, currentTile.Position.y, z));
            currentTile = cameFrom[currentTile];
        }
        
        return new Queue<Vector3>(path.Reverse());
    }
}
