using System;
using System.Collections;
using MayBe;
using UnityEngine;

public class TerrainMover : MonoBehaviour, IMover
{
    [SerializeField] 
    private GridEngine gridEngine;
    
    public float MoveSpeed = 10;
    
    private bool isMoving;
    public  bool IsMoving => isMoving;

    public event Action OnDone;

    private Path path;

    public void Move(Path path)
    {
        this.path = path;

        if (!isMoving)
            StartCoroutine(MoveCoroutine());
    }

    private IEnumerator MoveCoroutine()
    {
        isMoving = true;

        var previousTile = Maybe.None<MovementTile>();
        
        while (!path.ReachedEndPoint())
        {
            var nextPoint = path.Next();
            
            transform.up = nextPoint - transform.position;
            
            var nextTile = gridEngine.MovementGrid.GetTileAt(nextPoint);
            

            var calculatedSpeed = nextTile.Map(tile => tile.MovementCost)
                                          .Map(CalculateSpeedPenalty)
                                          .ValueOr(MoveSpeed);
            
            while (transform.position != nextPoint)
            {
                transform.position = Vector3.MoveTowards(transform.position, nextPoint, calculatedSpeed * Time.deltaTime);

                yield return new WaitForEndOfFrame();
            }
            
            nextTile.MatchSome(tile => tile.Block());
            previousTile.MatchSome(tile => tile.Unblock());
            previousTile = nextTile;
        }
        
        OnDone?.Invoke();

        isMoving = false;
    }
    
    private float CalculateSpeedPenalty(int movementCost)
    {
        return Movement.MaxCost - Mathf.Clamp(movementCost * movementCost, min: 0, max: Movement.MaxCost - 1);
    }
}
