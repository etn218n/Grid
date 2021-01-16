using GridSystem;
using UnityEngine;

public class Character : MonoBehaviour
{
    private Mover mover;
    private PathFinder pathFinder;

    public Mover Mover => mover;
    public PathFinder PathFinder => pathFinder;

    private void Awake()
    {
        mover = GetComponent<Mover>();
        pathFinder = GetComponent<PathFinder>();
    }

    public void Move<T>(BaseTile<T> source, BaseTile<T> destination) where T : BaseTile<T>, IHaveMovementCost
    {
        if (mover.IsMoving)
            return;

        var maybePath = pathFinder.CalculatePath(source, destination, transform.position.z);
        
        maybePath.MatchSome(path => mover.Move(path));
    }
}
