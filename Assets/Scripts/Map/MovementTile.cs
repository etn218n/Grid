using System;
using GridSystem;
using UnityEngine;

public class MovementTile : BaseTile<MovementTile>, IHaveMovementCost
{
    private int movementCost;
    public int MovementCost => movementCost;
    
    private bool isWalkable;
    public bool IsWalkable => isWalkable || MovementCost < Movement.MaxCost;

    public static Func<int, Rect2D> GenerateUVRectFromCost = (cost) => Rect2D.Zero; 
    
    public MovementTile(Grid<MovementTile> ownerGrid, Vector2Int coordinate) : base(ownerGrid, coordinate)
    {
    }

    public void Block()
    {
        isWalkable = false;
        SetUVs(GenerateUVRectFromCost(Movement.MaxCost));
    }

    public void Unblock()
    {
        isWalkable = true;
        SetUVs(GenerateUVRectFromCost(MovementCost));
    } 

    public void UpdateCost(int newCost)
    {
        movementCost = Mathf.Clamp(newCost, Movement.MinCost, Movement.MaxCost);
        SetUVs(GenerateUVRectFromCost(MovementCost));
    }

    public override bool IsOccupied => false;
    public override bool IsCollidable => false;
}