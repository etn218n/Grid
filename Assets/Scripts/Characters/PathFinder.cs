using Optional;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using GridSystem;
using PriorityQueue;

public class PathFinder : MonoBehaviour
{
    public enum PathfindingAlgorithm { AStar, BreadFirst, Dijkstra }

    public PathfindingAlgorithm Algorithm;

    public Option<Path> CalculatePath<T>(BaseTile<T> source, BaseTile<T> destination, float z)
        where T : BaseTile<T>, IHaveMovementCost
    {
        switch (Algorithm)
        {
            case PathfindingAlgorithm.AStar: return CalculateAStarPath(source, destination, z);
            case PathfindingAlgorithm.Dijkstra: return CalculateDijkstraPath(source, destination, z);
            case PathfindingAlgorithm.BreadFirst: return CalculateBreadthFirstPath(source, destination, z);
            default: return Option.None<Path>();
        }
    }

    public Option<Path> CalculateBreadthFirstPath<T>(BaseTile<T> source, BaseTile<T> destination, float z)
        where T : BaseTile<T>, IHaveMovementCost
    {
        var frontier = new Queue<BaseTile<T>>();
        var cameFrom = new Dictionary<BaseTile<T>, BaseTile<T>>();
        
        frontier.Enqueue(source);

        while (frontier.Any())
        {
            var current = frontier.Dequeue();
            
            if (current == destination)
                break;
            
            current.ForEachCardinalNeighbor(next =>
            {
                if (!cameFrom.ContainsKey(next) && next.IsWalkable)
                {
                    frontier.Enqueue(next);
                    cameFrom.Add(next, current);
                }
            });
        }

        return ConstructPath(cameFrom, source, destination, z);
    }

    public Option<Path> CalculateDijkstraPath<T>(BaseTile<T> source, BaseTile<T> destination, float z)
        where T : BaseTile<T>, IHaveMovementCost
    {
        var frontier  = new SimplePriorityQueue<BaseTile<T>>();
        var cameFrom  = new Dictionary<BaseTile<T>, BaseTile<T>>();
        var costSoFar = new Dictionary<BaseTile<T>, int>();
        
        costSoFar.Add(source, 0);
        frontier.Enqueue(source, 0);
        
        while (frontier.Any())
        {
            var current = frontier.Dequeue();
            
            if (current == destination)
                break;
            
            current.ForEachCardinalNeighbor(next =>
            {
                var newCost = costSoFar[current] + next.MovementCost;

                if ((!cameFrom.ContainsKey(next) || newCost < costSoFar[next]) && next.IsWalkable)
                {
                    costSoFar[next] = newCost;
                    frontier.Enqueue(next, newCost);
                    cameFrom[next] = current;
                }
            });
        }
        
        return ConstructPath(cameFrom, source, destination, z);
    }
    
    public Option<Path> CalculateAStarPath<T>(BaseTile<T> source, BaseTile<T> destination, float z)
        where T : BaseTile<T>, IHaveMovementCost
    {
        var frontier  = new SimplePriorityQueue<BaseTile<T>>();
        var cameFrom  = new Dictionary<BaseTile<T>, BaseTile<T>>();
        var costSoFar = new Dictionary<BaseTile<T>, int>();
        
        costSoFar.Add(source, 0);
        frontier.Enqueue(source, 0);
        
        while (frontier.Any())
        {
            var current = frontier.Dequeue();
            
            if (current == destination)
                break;
            
            current.ForEachCardinalNeighbor(next =>
            {
                var newCost = costSoFar[current] + next.MovementCost;

                if ((!cameFrom.ContainsKey(next) || newCost < costSoFar[next]) && next.IsWalkable)
                {
                    costSoFar[next] = newCost;
                    frontier.Enqueue(next, newCost + HeuristicDistance(next.Position, destination.Position));
                    cameFrom[next] = current;
                }
            });
        }
        
        return ConstructPath(cameFrom, source, destination, z);
    }

    private Option<Path> ConstructPath<T>(Dictionary<BaseTile<T>, BaseTile<T>> cameFrom, BaseTile<T> source, BaseTile<T> destination, float z) 
        where T : BaseTile<T>, IHaveMovementCost
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

    public float HeuristicDistance(Vector2 a, Vector2 b)
    {
        return Mathf.Abs(a.x + b.x) + Mathf.Abs(a.y + b.y);
    }
}
