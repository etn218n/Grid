using GridSystem;
using UnityEngine;

public class MovementTile : BaseTile<MovementTile>, IHaveMovementCost
{
    private short movementCost;
    public  short MovementCost => movementCost;
    
    public MovementTile(Grid<MovementTile> ownerGrid, Chunk ownerChunk, Vector2Int coordinate, Vector2Int localCoordinate) : base(ownerGrid, ownerChunk, coordinate, localCoordinate)
    {
    }

    public void UpdateCost(short newCost) => movementCost = newCost;
}