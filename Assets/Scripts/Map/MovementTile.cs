using GridSystem;
using UnityEngine;

public class MovementTile : BaseTile<MovementTile>, IHaveMovementCost
{
    private int movementCost;
    public  int MovementCost => movementCost;
    
    public MovementTile(Grid<MovementTile> ownerGrid, Vector2Int coordinate) : base(ownerGrid, coordinate)
    {
    }

    public void UpdateCost(int newCost) => movementCost = newCost;
    public override bool IsOccupied => false;
    public override bool IsCollidable => false;
}