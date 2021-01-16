public interface IHaveMovementCost
{
    int MovementCost { get; }
    bool IsWalkable { get; }
}

public static class Movement
{
    public const int MinCost = 0;
    public const int MaxCost = 10;
}