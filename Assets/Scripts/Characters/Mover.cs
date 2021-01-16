using System;
using UnityEngine;
using System.Collections;

public class Mover : MonoBehaviour, IMover
{
    public float MoveSpeed;

    private bool isMoving;
    public  bool IsMoving => isMoving;

    public event Action OnDone; 

    public void Move(Path path)
    {
        if (isMoving)
            return;
        
        StartCoroutine(MoveCoroutine(path));
    }

    private IEnumerator MoveCoroutine(Path path)
    {
        isMoving = true;
        
        while (!path.ReachedEndPoint())
        {
            var nextPoint = path.Next();
            
            transform.up = nextPoint - transform.position;

            while (transform.position != nextPoint)
            {
                transform.position = MoveTowards(nextPoint, MoveSpeed);

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
}
