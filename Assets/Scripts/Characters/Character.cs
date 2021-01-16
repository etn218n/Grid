using GridSystem;
using UnityEngine;

public class Character : MonoBehaviour
{
    private IMover mover;
    private PathFinder pathFinder;

    public IMover Mover => mover;
    public PathFinder PathFinder => pathFinder;

    private void Awake()
    {
        mover = GetComponent<IMover>();
        pathFinder = GetComponent<PathFinder>();
    }

    public void Move<T>(BaseTile<T> source, BaseTile<T> destination) where T : BaseTile<T>, IHaveMovementCost
    {
        var maybePath = pathFinder.CalculatePath(source, destination, transform.position.z);
        
        maybePath.MatchSome(path => mover.Move(path));
    }
}
