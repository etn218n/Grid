using System;
using GridSystem;
using UnityEngine;

[Serializable]
public class CameraZoomer : ICameraAction
{
    [SerializeField] private float zoomScale = 1;

    public void PerformOnCamera(Camera camera)
    {
        float cameraOrthographicSize = camera.orthographicSize;
        
        if (Input.GetAxis("Mouse ScrollWheel") > 0f)
            cameraOrthographicSize -= zoomScale;
        else if (Input.GetAxis("Mouse ScrollWheel") < 0f)
            cameraOrthographicSize += zoomScale;

        camera.orthographicSize = Mathf.Clamp(cameraOrthographicSize, 1, 100);
    }
}