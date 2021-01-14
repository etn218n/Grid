using System;
using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using GridSystem;
using Optional;

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
                        var path = CalculatePath(source, destination, character.transform.position.z, engine.MovementGrid.TileSize);
                        character.Move(path);
                    });
                });
            });
        }
    }

    private Queue<Vector3> CalculatePath(Vector3 source, Vector3 destination, float stepSize)
    {
        Queue<Vector3> path = new Queue<Vector3>();

        var horizontalSteps = (int)(Mathf.Abs(destination.x - source.x) / stepSize);
        var verticalSteps   = (int)(Mathf.Abs(destination.y - source.y) / stepSize);

        var up    = (destination.y - source.y) > 0 ? 1 : -1;
        var right = (destination.x - source.x) > 0 ? 1 : -1;

        float farthestX = 0;

        for (int i = 1; i <= horizontalSteps; i++)
        {
            if (i == horizontalSteps)
                farthestX = source.x + i * stepSize * right;
            
            path.Enqueue(new Vector3(source.x + i * stepSize * right, source.y, source.z));
        }

        for (int i = 1; i <= verticalSteps; i++)
            path.Enqueue(new Vector3(farthestX, source.y + i * stepSize * up, source.z));

        return path;
    }
    
    private Queue<Vector3> CalculatePath(MovementTile source, MovementTile destination, float z, float stepSize)
    {
        Queue<Vector3> path = new Queue<Vector3>();

        var horizontalSteps = (int)(Mathf.Abs(destination.Position.x - source.Position.x) / stepSize);
        var verticalSteps   = (int)(Mathf.Abs(destination.Position.y - source.Position.y) / stepSize);

        var up    = (destination.Position.y - source.Position.y) > 0;
        var right = (destination.Position.x - source.Position.x) > 0;

        var neighbor = right ? source.EastNeighbor : source.WestNeighbor;
        
        if (right)
        {
            while (neighbor.HasValue)
            {
                neighbor = neighbor.Filter(n => n.MovementCost != 10).FlatMap(n => n.EastNeighbor);
                neighbor.MatchSome(n => path.Enqueue(new Vector3(n.Position.x, n.Position.y, z)));
            }
            for (int i = 1; i < horizontalSteps; i++)
            {
                
            }
        }
        else
        {
            for (int i = 1; i < horizontalSteps; i++)
            {
                neighbor = neighbor.FlatMap(n => n.WestNeighbor);
                neighbor.MatchSome(n => path.Enqueue(new Vector3(n.Position.x, n.Position.y, z)));
            }
        }

        if (up)
        {
            for (int i = 1; i <= verticalSteps; i++)
            {
                neighbor = neighbor.FlatMap(n => n.NorthNeighbor);
                neighbor.MatchSome(n => path.Enqueue(new Vector3(n.Position.x, n.Position.y, z)));
            }
        }
        else
        {
            for (int i = 1; i <= verticalSteps; i++)
            {
                neighbor = neighbor.FlatMap(n => n.SouthNeighbor);
                neighbor.MatchSome(n => path.Enqueue(new Vector3(n.Position.x, n.Position.y, z)));
            }
        }

        return path;
    }
}
