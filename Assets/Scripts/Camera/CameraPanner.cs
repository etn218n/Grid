using System;
using GridSystem;
using Sirenix.OdinInspector;
using UnityEngine;

[Serializable]
public class CameraPanner : ICameraAction
{
    [SerializeField] [ReadOnly]
    private GrabPoint grabPoint = new GrabPoint();

    public void PerformOnCamera(Camera camera)
    {
        if (Input.GetMouseButtonDown(2))
        {
            grabPoint.Grab(camera.ScreenToWorldPoint(Input.mousePosition));
        }
        else if (Input.GetMouseButtonUp(2))
        {
            grabPoint.Ungrab();
        }

        if (grabPoint.IsGrabbed)
        {
            Vector3 mousePosition = camera.ScreenToWorldPoint(Input.mousePosition);

            Vector3 moveDirection = grabPoint.GrabPosition - mousePosition;

            camera.transform.Translate(moveDirection);
        }
    }

    [Serializable]
    private class GrabPoint
    {
        private bool isGrabbed;
        private Vector3 grabPosition;

        public bool IsGrabbed => isGrabbed;
        public Vector3 GrabPosition => grabPosition;

        public void Grab(Vector3 grabPoint)
        {
            isGrabbed = true;
            grabPosition = grabPoint;
        }

        public void Ungrab()
        {
            isGrabbed = false;
        }
    }
}