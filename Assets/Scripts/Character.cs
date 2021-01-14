using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class Character : MonoBehaviour
{
    public float MoveSpeed;

    private bool isMoving;
    
    public void Move(Vector3 destination)
    {
        if (isMoving)
            return;
        
        StartCoroutine(MoveCoroutine(destination));
    }
    
    public void Move(Queue<Vector3> path)
    {
        if (isMoving)
            return;
        
        StartCoroutine(MoveCoroutine(path));
    }
    
    private IEnumerator MoveCoroutine(Vector3 destination)
    {
        isMoving = true;

        destination.z = transform.position.z;

        transform.up = destination - transform.position;

        while (transform.position != destination)
        {
            transform.position = Vector3.MoveTowards(transform.position, destination, MoveSpeed * Time.deltaTime);

            yield return new WaitForEndOfFrame();
        }

        isMoving = false;
    }
    
    private IEnumerator MoveCoroutine(Queue<Vector3> path)
    {
        isMoving = true;
        
        while (path.Any())
        {
            var nextPoint = path.Dequeue();
            
            transform.up = nextPoint - transform.position;

            while (transform.position != nextPoint)
            {
                transform.position = Vector3.MoveTowards(transform.position, nextPoint, MoveSpeed * Time.deltaTime);

                yield return new WaitForEndOfFrame();
            }
        }

        isMoving = false;
    }
}
