using System;
using GridSystem;
using UnityEngine;

[Serializable]
public class CameraZoomer : ICameraAction
{
    [SerializeField] private float zoomScale = 1;

    public void PerformOnCamera(Camera camera)
    {
        if (Input.GetAxis("Mouse ScrollWheel") > 0f)
            camera.orthographicSize -= zoomScale;
        else if (Input.GetAxis("Mouse ScrollWheel") < 0f)
            camera.orthographicSize += zoomScale;
    }
}