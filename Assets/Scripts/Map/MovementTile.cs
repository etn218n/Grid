using GridSystem;
using UnityEngine;

public class MovementTile : BaseTile<MovementTile>, IHaveMovementCost
{
    private short movementCost;
    public  short MovementCost => movementCost;
    
    public MovementTile(Grid<MovementTile> ownerGrid, Vector2Int coordinate) : base(ownerGrid, coordinate)
    {
    }

    public void UpdateCost(short newCost) => movementCost = newCost;
    public override bool IsOccupied => false;
    public override bool IsCollidable => false;
}