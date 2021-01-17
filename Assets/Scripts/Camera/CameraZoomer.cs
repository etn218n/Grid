using GridSystem;
using UnityEngine;

public class CameraZoomer : CameraAction
{
    [SerializeField] private float zoomScale = 1;
    [SerializeField] private float minZoomDistance = 1;
    [SerializeField] private float maxZoomDistance = 100;

    public override void Perform(Camera camera)
    {
        float cameraOrthographicSize = camera.orthographicSize;
        
        if (Input.GetAxis("Mouse ScrollWheel") > 0f)
            cameraOrthographicSize -= zoomScale;
        else if (Input.GetAxis("Mouse ScrollWheel") < 0f)
            cameraOrthographicSize += zoomScale;

        camera.orthographicSize = Mathf.Clamp(cameraOrthographicSize, minZoomDistance, maxZoomDistance);
    }
}