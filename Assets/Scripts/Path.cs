using System;
using UnityEngine;
using System.Collections.Generic;

public class Path
{
    private List<Vector3> points = new List<Vector3>();
    private int currentIndex = 0;
    public int Count => points.Count;

    public bool ReachedEndPoint() => currentIndex == points.Count;
    
    public void AddPoint(Vector3 point) => points.Add(point);
    public void AddPoint(float x, float y, float z) => points.Add(new Vector3(x, y, z));
    public void RemovePoint(Vector3 point) => points.Remove(point);
    
    public void Reverse() => points.Reverse();
    public void ForEachPoint(Action<Vector3> action) => points.ForEach(p => action(p));

    public void Reset() => currentIndex = 0;

    public Vector3 Next()
    {
        if (currentIndex >= points.Count)
            Reset();

        return points[currentIndex++];
    }
}