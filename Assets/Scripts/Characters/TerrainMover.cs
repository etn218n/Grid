using System;
using System.Collections;
using UnityEngine;

public class TerrainMover : MonoBehaviour, IMover
{
    [SerializeField] private GridEngine gridEngine;
    
    public float DefaultMoveSpeed;
    
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
        
        while (!path.ReachedEndPoint())
        {
            var nextPoint = path.Next();
            
            transform.up = nextPoint - transform.position;

            float movementSpeed = gridEngine.MovementGrid.GetTileAt(nextPoint)
                .Map(tile => tile.MovementCost)
                .Map(MovementSpeed)
                .ValueOr(DefaultMoveSpeed);

            while (transform.position != nextPoint)
            {
                transform.position = MoveTowards(nextPoint, movementSpeed);

                yield return new WaitForEndOfFrame();
            }
        }
        
        OnDone?.Invoke();

        isMoving = false;

        Vector3 MoveTowards(Vector3 nextPoint, float moveSpeed)
        {
            return Vector3.MoveTowards(transform.position, nextPoint, moveSpeed * Time.deltaTime);
        }
    }

    /**
     * Higher cost, lower speed
     */
    private float MovementSpeed(int movementCost)
    {
        return Movement.MaxCost - Mathf.Clamp(movementCost * movementCost, min: 0, max: Movement.MaxCost - 1);
    }
}
