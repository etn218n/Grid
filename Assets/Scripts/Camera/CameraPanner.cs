using GridSystem;
using UnityEngine;

public class CameraPanner : CameraAction
{
    private GrabPoint grabPoint = new GrabPoint();

    public override void Perform(Camera camera)
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

            Vector3 moveDirection = grabPoint.Position - mousePosition;

            camera.transform.Translate(moveDirection);
        }
    }

    private class GrabPoint
    {
        public bool IsGrabbed { get; private set; }
        public Vector3 Position { get; private set; }

        public void Grab(Vector3 grabPoint)
        {
            IsGrabbed = true;
            Position  = grabPoint;
        }

        public void Ungrab()
        {
            IsGrabbed = false;
        }
    }
}