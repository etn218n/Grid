using System;
using System.Collections;
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
        
        while (!path.ReachedEndPoint())
        {
            var nextPoint = path.Next();
            
            transform.up = nextPoint - transform.position;

            float calculatedSpeed = gridEngine.MovementGrid.GetTileAt(nextPoint).Map(tile => tile.MovementCost)
                                                                                .Map(CalculateSpeedPenalty)
                                                                                .ValueOr(MoveSpeed);
            
            while (transform.position != nextPoint)
            {
                transform.position = Vector3.MoveTowards(transform.position, nextPoint, calculatedSpeed * Time.deltaTime);

                yield return new WaitForEndOfFrame();
            }
        }
        
        OnDone?.Invoke();

        isMoving = false;
    }
    
    private float CalculateSpeedPenalty(int movementCost)
    {
        return Movement.MaxCost - Mathf.Clamp(movementCost * movementCost, min: 0, max: Movement.MaxCost - 1);
    }
}
